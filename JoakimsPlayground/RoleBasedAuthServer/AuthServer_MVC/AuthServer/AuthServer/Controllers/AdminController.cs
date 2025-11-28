using AuthServer.Data.Context;
using AuthServer.Models.Admin;
using AuthServer.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
	public class AdminController : Controller
	{
		private readonly ILogger<AdminController> _logger;

		private readonly AuthServerDbContext _context;

		private readonly IAdminService _adminService;

		public AdminController(ILogger<AdminController> logger, AuthServerDbContext context, IAdminService adminService)
		{
			_logger = logger;
			_context = context;
			_adminService = adminService;
		}

		public IActionResult AdminIndex()
		{
			var usersTest= _context.Users.OrderBy(x => x.NormalizedUserName).Skip(0).Take(10).ToList();

			var users = usersTest.Select(x => new UserOverviewDto
			{
				Id = x.Id,
				UserName = x.UserName,
				Email = x.Email
			}).ToList();

			return View(users);
		}

		public async Task<IActionResult> EditUser(string id, string? message)
		{
			var user = await _adminService.GetUserAsync(id);

			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "User not found.");
				return View();
			}
			var editUserDto = new EditUserDto
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email
			};

			return View(editUserDto);
		}

		public async Task<IActionResult> EditUserForm(EditUserDto editUserDto)
		{
			var (status, message) = await _adminService.UpdateUserAsync(editUserDto);

			if (status == 0)
			{
				ModelState.AddModelError(string.Empty, message);
				return View(editUserDto);
			}

			TempData["SuccessMessage"] = message;
			return RedirectToAction($"EditUser", new { id = editUserDto.Id });
		}
	}
}
