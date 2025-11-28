using AuthServer.Data.Context;
using AuthServer.Data.UserManagement;
using AuthServer.Models.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Services.Admin
{
	public class AdminService : IAdminService
	{

		private readonly AuthServerDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;

		public AdminService(AuthServerDbContext context,
							UserManager<ApplicationUser> userManager,
						    RoleManager<IdentityRole> roleManager,
						    IConfiguration configuration)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
		}

		public async Task<(int, string)> UpdateUserAsync(EditUserDto editUserDto)
		{
			if (editUserDto == null ||
				editUserDto.Id.IsNullOrEmpty() ||
				editUserDto.UserName.IsNullOrEmpty() ||
				editUserDto.Email.IsNullOrEmpty())
			{
				return (0, "Bad request to edit user. Check your data");
			}

			var user = _context.Users.SingleOrDefault(x => x.Id == editUserDto.Id);

			if (user == null)
			{
				return (0, "User not found.");
			}

			user.UserName = editUserDto.UserName;
			user.Email = editUserDto.Email;

			if (editUserDto.NewPassword != null &&
				editUserDto.NewPassword == editUserDto.ConfirmedNewPassword)
			{
				var newPassword = _userManager.PasswordHasher.HashPassword(user, editUserDto.NewPassword);
				user.PasswordHash = newPassword;
			}

			_context.Update(user);
			_context.SaveChanges();

			return (1, "User successfully updated.");
		}

		public async Task<ApplicationUser> GetUserAsync(string? id) => 
			await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
	}
}
