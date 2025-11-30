using Hassecore.API.Data.Entities;
using Hassecore.API.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hassecore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IBaseRepository _baseRepository;

        private readonly ILogger<WeatherForecastController> _logger;

        public UsersController(IBaseRepository repository,
                                          ILogger<WeatherForecastController> logger)
        {
            _baseRepository = repository;
            _logger = logger;
        }

        [HttpGet]
        [Route("Get")]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _baseRepository.GetAsync<User>(id);

            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found.", id);
                return NotFound();
            }
            else
            {
                _logger.LogInformation("User with id {UserId} retrieved successfully.", id);
                return Ok(user);
            }
        }

        [HttpGet]
        [Route("request-pairing/{externalId}")]
        public async Task<IActionResult> RequestPairing(string externalId) // external Id should be replaced with actual Id from the db of the API.
        {
            var user = await _baseRepository.GetSingleOrDefaultAsync<User>(x => x.ExternalId == externalId);

            // Implement logic to begin pairing users here.

            _logger.LogInformation("User with id {UserId} retrieved successfully.", externalId);
            return Ok(user);
        }
    }
}
