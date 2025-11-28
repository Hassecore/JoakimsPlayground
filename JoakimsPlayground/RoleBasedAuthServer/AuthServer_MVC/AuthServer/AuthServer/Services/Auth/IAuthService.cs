using AuthServer.Models;

namespace AuthServer.Services.Auth
{
	public interface IAuthService
	{
		Task<(int, string, string)> HandleGoogleUserAsync(ExternalUserDto userDto);
		Task<(int, string, string)> RegisterAsync(RegisterDto model, string roleName);
		Task<(int, string)> AuthorizeAsync(LoginDto loginDto);
		//Task<List<ApplicationUser>> GetUsers();
		Task<(int, string)> Token(string authorizationCode);
		//string GenerateAuthorizationCode(string userName);
		bool ValidateCodeVerifier(string codeVerifier);
	}
}
