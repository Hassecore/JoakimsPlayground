using AuthServer.Data.UserManagement;
using AuthServer.Models;
using AuthServer.Models.Enums;
using AuthServer.Services.Caching;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthServer.Services.Auth
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
		private readonly ICachingService _cachingService;

		public AuthService(UserManager<ApplicationUser> userManager,
						   RoleManager<IdentityRole> roleManager,
						   SignInManager<ApplicationUser> signInManager,
                           IConfiguration configuration,
						   ICachingService cachingService)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
            _configuration = configuration;
			_cachingService = cachingService;

		}

		public async Task<(int, string, string)> HandleGoogleUserAsync(ExternalUserDto userDto)
		{
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
			if (existingUser == null)
			{
				return await RegisterGoogleUser(userDto, UserRoles.User.ToString());
			}

			var authorizationCode = GenerateAuthorizationCode(existingUser.Id);

			return (1, "User created successfully!", authorizationCode);
		}

		private async Task<(int, string, string)> RegisterGoogleUser(ExternalUserDto userDto, string roleName)
		{
            var userNameExists = await _userManager.FindByEmailAsync(userDto.UserName);

            var emailExists = await _userManager.FindByEmailAsync(userDto.Email);
            if (emailExists != null || userNameExists != null)
            {
                return (0, "Email already exists!", string.Empty);
            }

            ApplicationUser user = new()
            {
                Email = userDto.Email,
                UserName = userDto.UserName,
                //FirstName = model.FirstName,
                //LastName = model.LastName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createUserResult = await _userManager.CreateAsync(user);

            if (!createUserResult.Succeeded)
            {
                return (0, "User creation failed! Please check user details and try again.", string.Empty);
            }

			var addUserLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(EnumIdentityProviders.Google.ToString(), userDto.ExternalUserId, EnumIdentityProviders.Google.ToString()));
            if (!addUserLoginResult.Succeeded)
            {
                return (0, "Failed to assign external info to user.! Please check user details and try again.", string.Empty);
            }

            await GiveUserRole(user, roleName);

            var authorizationCode = GenerateAuthorizationCode(user.Id);

            return (1, "User created successfully!", authorizationCode);
        }

        public async Task<(int, string, string)> RegisterAsync(RegisterDto model, string roleName)
		{
			var userNameExists = await _userManager.FindByNameAsync(model.UserName);
			if (userNameExists != null)
			{
				return (0, "Username already exists!", string.Empty);
			}

			var emailExists = await _userManager.FindByEmailAsync(model.Email);
			if (emailExists != null)
			{
				return (0, "Email already exists!", string.Empty);
			}

			if (model.Password != model.ConfirmPassword)
			{
				return (0, "Passwords must match!", string.Empty);
			}

			ApplicationUser user = new()
			{
				Email = model.Email,
				UserName = model.UserName,
				//FirstName = model.FirstName,
				//LastName = model.LastName,
				SecurityStamp = Guid.NewGuid().ToString(),
			};

			var createUserResult = string.IsNullOrEmpty(model.Password) ? 
								   await _userManager.CreateAsync(user) : 
								   await _userManager.CreateAsync(user, model.Password);

			if (!createUserResult.Succeeded)
			{
				return (0, "User creation failed! Please check user details and try again.", string.Empty);
			}

			await GiveUserRole(user, roleName);
            await _userManager.AddLoginAsync(user, new UserLoginInfo(EnumIdentityProviders.Local.ToString(), user.Id, EnumIdentityProviders.Local.ToString()));


            var authorizationCode = GenerateAuthorizationCode(user.Id);

			return (1, "User created successfully!", authorizationCode);

		}

		public async Task GiveUserRole(ApplicationUser user, string roleName)
		{
			if (!await _roleManager.RoleExistsAsync(roleName))
			{
				await _roleManager.CreateAsync(new IdentityRole(roleName));
			}

			if (await _roleManager.RoleExistsAsync(roleName))
			{
				await _userManager.AddToRoleAsync(user, roleName);
			}
			else
			{
				throw new InvalidOperationException("Role creation failed");
			}
		}

		public async Task<IdentityResult> CreateLocalUserAsync(ApplicationUser user, string password)
		{
			return await _userManager.CreateAsync(user, password);
		}

		public async Task<IdentityResult> CreateExternalUserAsync(ApplicationUser user, string password)
		{
			return await _userManager.CreateAsync(user);
		}

		public async Task<(int, string)> AuthorizeAsync(LoginDto model)
		{
			var user = await _userManager.FindByNameAsync(model.UserName);
			if (user == null)
			{
				return (0, "Invalid username or password");
			}

			if (!await _userManager.CheckPasswordAsync(user, model.Password))
			{
				return (0, "Invalid password or password.");
			}

			var authorizationCode = GenerateAuthorizationCode(user.Id);

			return (1, authorizationCode);
		}

		public async Task<(int, string)> Token(string authorizationCode)
		{
			// Add cache check here.

			var userNameObj = _cachingService.Get<string>(authorizationCode);
			if (userNameObj == null)
			{
				return (0, string.Empty);
			}
			var userName = userNameObj.ToString();
			var user = await _userManager.FindByIdAsync(userName);
			if (user == null)
			{
				return (0, string.Empty);
			}
            var authClaims = new List<Claim>
			{
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Name, user.UserName),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

            var userLogins = await _userManager.GetLoginsAsync(user);
			if (userLogins.Count > 0)
			{
				foreach (var userLogin in userLogins)
				{
					authClaims.Add(new Claim($"{userLogin.ProviderDisplayName.ToLowerInvariant()}_id", userLogin.ProviderKey));
                }
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, userRole));
			}

			return (1, GenerateToken(authClaims));
		}

		public bool ValidateCodeVerifier(string codeVerifier)
		{
			var codeChallenge = GenerateCodeChallenge(codeVerifier);

			var result = _cachingService.Get<string>(codeChallenge);

			return (result != null);
		}

		public static string GenerateCodeVerifier()
		{
			// 32 random bytes = 43-character base64url string
			var bytes = new byte[32];
			RandomNumberGenerator.Fill(bytes);

			string base64 = Convert.ToBase64String(bytes);

			// Convert to Base64 URL format
			return base64
				.Replace("+", "-")
				.Replace("/", "_")
				.Replace("=", ""); // PKCE requires no padding
		}

		public static string GenerateCodeChallenge(string codeVerifier)
		{
			using var sha256 = SHA256.Create();
			var bytes = Encoding.ASCII.GetBytes(codeVerifier);
			var hash = sha256.ComputeHash(bytes);

			string base64 = Convert.ToBase64String(hash);

			// Convert to Base64 URL format
			return base64
				.Replace("+", "-")
				.Replace("/", "_")
				.Replace("=", "");
		}

		//public async Task<List<ApplicationUser>> GetUsers()
		//{
		//	var users = await _userManager.GetUsersInRoleAsync("User");

		//	return users.ToList();
		//}

		private string GenerateAuthorizationCode(string userId)
		{
			var bytes = new byte[32]; // 256-bit random
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(bytes);
			}
			var result = Convert.ToBase64String(bytes)
						  .TrimEnd('=')
						  .Replace('+', '-')
						  .Replace('/', '_'); // URL-safe

			var expiryTimeInSeconds = Convert.ToInt32(_configuration["Cache:TokenExpiryTimeInSeconds"]);

			_cachingService.Set(result, userId, new TimeSpan(0, 0, expiryTimeInSeconds));
			return result;
		}

		private string GenerateToken(IEnumerable<Claim> claims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
			var tokenExpiryInMinutes = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInMinutes"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = _configuration["JWTKey:ValidIssuer"],
				Audience = _configuration["JWTKey:ValidAudience"],
				Expires = DateTime.UtcNow.AddMinutes(tokenExpiryInMinutes),
				SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
				Subject = new ClaimsIdentity(claims)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
	}
}
