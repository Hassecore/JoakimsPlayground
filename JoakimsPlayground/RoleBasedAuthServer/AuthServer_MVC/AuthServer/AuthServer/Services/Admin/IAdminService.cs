using AuthServer.Data.UserManagement;
using AuthServer.Models.Admin;

namespace AuthServer.Services.Admin
{
	public interface IAdminService
	{
		Task<(int, string)> UpdateUserAsync(EditUserDto editUserDto);
		Task<ApplicationUser> GetUserAsync(string? id);
	}
}
