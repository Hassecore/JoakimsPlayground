using Hassecore.API.Business.Managers;
using MediatR;

namespace Hassecore.API.Business.MediatR.UserPairing
{
    internal class DenyPairingCommandHandler : IRequestHandler<DenyPairingCommand, bool>
    {
        private readonly IUserPairingService _userPairingService;
        public DenyPairingCommandHandler(IUserPairingService userPairingService)
        {
            _userPairingService = userPairingService;
        }

        public async Task<bool> Handle(DenyPairingCommand request, CancellationToken cancellationToken)
        {
            return await _userPairingService.DenyUserPairingRequestAsync(request.DenyingUserId, request.UserPairingRequestId);
        }
    }
}
