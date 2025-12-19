using AuthServer.Data.Context;
using AuthServer.Data.UserManagement;
using AuthServer.Services.Admin;
using AuthServer.Services.Auth;
using AuthServer.Services.Caching;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			//builder.Services.AddDataProtection()
			//				.PersistKeysToFileSystem(new DirectoryInfo(@"./keys/"))
			//				.SetApplicationName("AuthServer");

			builder.Services.AddScoped<ICachingService, CachingService>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<IAdminService, AdminService>();

			//builder.Services.AddDbContext<AuthServerDbContext>(options =>
			//	options.UseInMemoryDatabase("AuthServerDb"));

            builder.Services.AddDbContext<AuthServerDbContext>(options =>
			options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
                )
			);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
							.AddEntityFrameworkStores<AuthServerDbContext>()
							.AddDefaultTokenProviders();


			builder.Services.AddAuthentication(
				options =>
				{
					//options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					//options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					//options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

				}
			)
				//.AddJwtBearer(options =>
				//{
				//	options.SaveToken = true;
				//	options.RequireHttpsMetadata = false;
				//	options.TokenValidationParameters = new TokenValidationParameters()
				//	{
				//		ValidateIssuer = true,
				//		ValidateAudience = true,
				//		ValidAudience = builder.Configuration["JWTKey:ValidAudience"],
				//		ValidIssuer = builder.Configuration["JWTKey:ValidIssuer"],
				//		ClockSkew = TimeSpan.Zero,
				//		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTKey:Secret"]))
				//	};
				//.AddCookie(IdentityConstants.ApplicationScheme)
			.AddGoogle(options =>
			{
				var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
				var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
				if (string.IsNullOrEmpty(googleClientId) ||
					string.IsNullOrEmpty(googleClientSecret))
				{
					throw new InvalidDataException("Google Client Id/Secret is not configured.");
				}

				options.ClientId = googleClientId;
				options.ClientSecret = googleClientSecret;
				options.CallbackPath = "/signin-google";
				options.SignInScheme = IdentityConstants.ExternalScheme;
				options.SaveTokens = true;
			});

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Auth}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
