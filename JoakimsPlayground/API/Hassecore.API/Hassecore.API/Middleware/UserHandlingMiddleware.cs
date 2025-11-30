using Hassecore.API.Data.Entities;
using Hassecore.API.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Hassecore.API.Middleware
{
    public class UserHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public UserHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IBaseRepository repository)
        {
            // Add user handling logic here
            //var test = context.User.Identity.Name;
            //var test2 = repository.GetAsync<User>(Guid.NewGuid());
            var userUniqueId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userUniqueId != null)
            {
                var existingUser = (await repository.GetSingleOrDefaultAsync<User>(x => x.ExternalId == userUniqueId));
                if (existingUser == null)
                {
                    var userCreatedSuccessfully = await CreateUser(context, repository);
                    if (!userCreatedSuccessfully)
                    {
                        return;
                    }
                }
            }
            await _next(context);
        }

        private async Task<bool> CreateUser(HttpContext context, IBaseRepository repository)
        {
            var timeNow = DateTime.UtcNow;
            var externalId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = context.User?.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
            var email = context.User?.FindFirst(ClaimTypes.Email)?.Value;

            if (!UserDataIsValid(externalId, username, email))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return false;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Username = username,
                Email = email,
                CreatedAt = timeNow,
                UpdatedAt = timeNow,
                LastOnline = DateOnly.FromDateTime(timeNow)
            };

            await repository.CreateAsync(user);

            return true;
        }

        private bool UserDataIsValid(string? externalId, string? userName, string? email)
        {
            if (externalId.IsNullOrEmpty() ||
                userName.IsNullOrEmpty() ||
                email.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }
    }
}
