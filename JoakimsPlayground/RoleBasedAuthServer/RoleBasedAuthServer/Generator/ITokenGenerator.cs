using System.Security.Claims;

namespace RoleBasedAuthServer.Generator
{
	public interface ITokenGenerator
	{
		string GenerateToken(IEnumerable<Claim> claims);
	}
}
