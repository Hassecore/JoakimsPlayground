using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RoleBasedAuthServer.Models;
using RoleBasedAuthServer.UserManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RoleBasedAuthServer.AuthServices
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;

		public AuthService(UserManager<ApplicationUser> userManager,
						   RoleManager<IdentityRole> roleManager,
						   IConfiguration configuration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
		}

		public async Task<(int, string)> Registration(RegistrationModel model, string roleName)
		{
			var userExists = await _userManager.FindByNameAsync(model.UserName);
			if (userExists != null)
			{
				return (0, "User already exists");
			}

			ApplicationUser user = new()
			{
				Email = model.Email,
				UserName = model.UserName,
				//FirstName = model.FirstName,
				//LastName = model.LastName,
				SecurityStamp = Guid.NewGuid().ToString(),
				//PasswordHash = HashPassword2(model.Password)
			};

			var createUserResult = await _userManager.CreateAsync(user, model.Password);
			if (!createUserResult.Succeeded)
			{
				return (0, "User createion failed! Please check user details and try again.");
			}

			if (!await _roleManager.RoleExistsAsync(roleName))
			{
				await _roleManager.CreateAsync(new IdentityRole(roleName));
			}

			if (await _roleManager.RoleExistsAsync(roleName))
			{
				await _userManager.AddToRoleAsync(user, roleName);
			}

			return (1, "User created successfully!");

		}

		public async Task<(int, string)> Login(LoginModel model)
		{
			var user = await _userManager.FindByNameAsync(model.UserName);
			if (user == null)
			{
				return (0, "Invalid username");
			}

			if (!await _userManager.CheckPasswordAsync(user, model.Password))
			{
				return (0, "Invalid password.");
			}

			var userRoles = await _userManager.GetRolesAsync(user);
			var authClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			foreach (var userRole in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, userRole));
			}

			string token = GenerateToken(authClaims);
			return (1, token);
		}

		private string GenerateToken(IEnumerable<Claim> claims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
			var tokenExpiryInHours = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHours"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = _configuration["JWTKey:ValidIssuer"],
				Audience = _configuration["JWTKey:ValidAudience"],
				Expires = DateTime.UtcNow.AddMinutes(10),
				SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
				Subject = new ClaimsIdentity(claims)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
	}
}
