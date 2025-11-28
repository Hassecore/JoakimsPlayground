using RoleBasedAuthServer.Models;

namespace RoleBasedAuthServer.AuthServices
{
	public interface IAuthService
	{
		Task<(int, string)> Registration(RegistrationModel model, string roleName);
		Task<(int, string)> Login(LoginModel model);
	}
}
