using MediatR;

namespace Hassecore.API.Business.MediatR.UserPairing
{
    public class DenyPairingCommand : IRequest<bool>
    {
        public required Guid UserPairingRequestId { get; set; }
        public required Guid DenyingUserId { get; set; }
    }
}
