using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoleBasedAuthServer.Generator
{
	public class TokenGenerator : ITokenGenerator
	{
		private IConfiguration _configuration;

		public TokenGenerator(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string GenerateToken(IEnumerable<Claim> claims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey.Secret"]));
			var tokenExpiryInHours = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHours"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = _configuration["JWTKey:TokenExpiryTimeInHours"],
				Audience = _configuration["JWTKey:TokenExpiryTimeInHours"],
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
