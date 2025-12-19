using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hassecore.API.Business.MediatR.UserPairing
{
    public class RequestPairingCommand : IRequest<bool>
    {
        public required Guid SenderId { get; set; }
        public required string ReceiverEmail { get; set; }
    }
}
