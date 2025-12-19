using Hassecore.API.Business.MediatR.UserPairing;
using Hassecore.API.Data.Entities;
using Hassecore.API.Data.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hassecore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IBaseRepository _baseRepository;

        private readonly ILogger<WeatherForecastController> _logger;

        public UsersController(IMediator mediator,
                               IBaseRepository repository,
                               ILogger<WeatherForecastController> logger)
        {
            _mediator = mediator;
            _baseRepository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync()
        {
            var users =  await _baseRepository.GetQueryable<User>(x => true).ToListAsync();
            
            _logger.LogInformation("Users retrieved successfully.");
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
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

        //[HttpGet]
        //[Route("{userName}")]
        //public async Task<IActionResult> SearchAsync(string email)
        //{
        //    var user = await _baseRepository.GetAsync<User>(id);

        //    if (user == null)
        //    {
        //        _logger.LogWarning("User with id {UserId} not found.", id);
        //        return NotFound();
        //    }
        //    else
        //    {
        //        _logger.LogInformation("User with id {UserId} retrieved successfully.", id);
        //        return Ok(user);
        //    }
        //}

        //[HttpPost]
        //[Route("request-pairing/{externalId}")]
        //public async Task<IActionResult> RequestPairingAsync([FromBody] RequestPairingCommand request) // external Id should be replaced with actual Id from the db of the API.
        //{
        //    var test = await _mediator.Send(request);
        //    //var test1 = await _mediator.Send(new AcceptPairingCommand());
        //    //var test2 = await _mediator.Send(new DenyPairingCommand());

        //    //var user = await _baseRepository.GetSingleOrDefaultAsync<User>(x => x.req == externalId);

        //    //// Implement logic to begin pairing users here.

        //    //_logger.LogInformation("User with id {UserId} retrieved successfully.", externalId);
        //    return Ok();
        //}
    }
}
