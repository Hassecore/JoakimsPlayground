using AuthServer.Data.UserManagement;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Data.Context
{
	public class AuthServerDbContext : IdentityDbContext<ApplicationUser>
	{
		//public DbSet<Expense> Expenses { get; set; }

		public AuthServerDbContext(DbContextOptions<AuthServerDbContext> options) : base(options)
		{

		}
	}
}
