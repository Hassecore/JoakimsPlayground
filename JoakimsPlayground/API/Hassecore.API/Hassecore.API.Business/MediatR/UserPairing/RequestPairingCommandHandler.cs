using Hassecore.API.Business.Managers;
using Hassecore.API.Data.Entities;
using Hassecore.API.Data.Repositories;
using MediatR;

namespace Hassecore.API.Business.MediatR.UserPairing
{
    public class RequestPairingCommandHandler : IRequestHandler<RequestPairingCommand, bool>
    {
        private readonly IUserPairingService _userPairingService;
        private readonly IBaseRepository _baseRepository;
        public RequestPairingCommandHandler(IUserPairingService userPairingService,IBaseRepository baseRepository)
        {
            _userPairingService = userPairingService;
            _baseRepository = baseRepository;
        }

        public async Task<bool> Handle(RequestPairingCommand request, CancellationToken cancellationToken)
        {
            var receivingUser = await _baseRepository.GetSingleOrDefaultAsync<User>(x => x.Email == request.ReceiverEmail);
            if (receivingUser == null ||
                receivingUser.Id == request.SenderId)
            {
                return false;
            }

            return await _userPairingService.CreateUserPairingRequestAsync(request.SenderId, receivingUser.Id);
        }
    }
}
