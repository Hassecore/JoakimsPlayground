using Hassecore.API.Business.MediatR.UserPairing;
using Hassecore.API.Data.Entities;
using Hassecore.API.Data.Repositories;
using Hassecore.API.DTOs.UserPairing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hassecore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class UserPairingController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IBaseRepository _baseRepository;

        private readonly ILogger<WeatherForecastController> _logger;

        public UserPairingController(IMediator mediator,
                               IBaseRepository repository,
                               ILogger<WeatherForecastController> logger)
        {
            _mediator = mediator;
            _baseRepository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Ping()
        {
            return Ok();
        }

        [HttpPost]
        [Route("request-pairing")]
        public async Task<IActionResult> RequestPairingAsync([FromBody] RequestUserPairingDto request)
        {
            string userExternalId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var userId = (await _baseRepository.GetSingleAsync<User>(x => x.ExternalId == userExternalId)).Id;

            var command = new RequestPairingCommand
            {
                ReceiverEmail = request.ReceiverEmail,
                SenderId = userId
            };
            var userPairingRequestWasCreated = await _mediator.Send(command);
            
            if (!userPairingRequestWasCreated)
            {
                return BadRequest("Pairing request could not be created. Users may already be paired or a request already exists.");
            }

            return Ok();
        }

        [HttpPost]
        [Route("accept-pairing")]
        public async Task<IActionResult> AcceptPairingAsync([FromBody] AcceptUserPairingDto request)
        {
            string userExternalId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var userId = (await _baseRepository.GetSingleAsync<User>(x => x.ExternalId == userExternalId)).Id;
            var command = new AcceptPairingCommand
            {
                UserPairingRequestId = request.UserPairingRequestId,
                AcceptingUserId = userId
            };

            var result = await _mediator.Send(command);

            if (!result)
            {
                return BadRequest();
            }
            
            return Ok();
        }

        [HttpDelete]
        [Route("deny-pairing")]
        public async Task<IActionResult> DenyPairingAsync([FromBody] DenyUserPairingDto request)
        {
            string userExternalId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var userId = (await _baseRepository.GetSingleAsync<User>(x => x.ExternalId == userExternalId)).Id;
            var command = new DenyPairingCommand
            {
                UserPairingRequestId = request.UserPairingRequestId,
                DenyingUserId = userId
            };

            var result = await _mediator.Send(command);

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet]
        [Route("user-pair/{id}")]
        public async Task<IActionResult> GetUserPairAsync(Guid id)
        {
            var userPair = await _baseRepository.GetAsync<UserPair>(id,
                                               x => x.User1,
                                               x => x.User2);

            return Ok(userPair);
        }
    }
}
