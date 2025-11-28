using JoakimsPlaygroundFunctions.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoakimsPlaygroundFunctions.Data.Contexts
{
	public class Context : DbContext
	{
		private const string DefaultSchema = "rando";

		public DbSet<Letter> Letters { get; set; }

		public Context()
		{
				
		}

		public Context(DbContextOptions<Context> dbContextOptions) : base(dbContextOptions)
		{
			
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer("Server=tcp:joakimssqlserver21.database.windows.net,1433;Initial Catalog=joakimTestDb5;Persist Security Info=False;User ID=TestUser;Password=2!@asd_ASoiu123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=180;",
											sqlServerOptions =>
											{
												sqlServerOptions.CommandTimeout(180);
											});
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema(DefaultSchema);

			modelBuilder.Entity<Letter>().HasKey(x => x.Id);
			modelBuilder.Entity<Letter>().Property(x => x.Sender).IsRequired();
			modelBuilder.Entity<Letter>().Property(x => x.Content).IsRequired();
			modelBuilder.Entity<Letter>().Property(x => x.PS).IsRequired(false);
		}

	}
}
