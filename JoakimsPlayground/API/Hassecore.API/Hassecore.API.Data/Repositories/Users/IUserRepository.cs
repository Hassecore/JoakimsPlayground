using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hassecore.API.Data.Repositories.Users
{
    public interface IUserRepository
    {
        bool RequestUserPairing(Guid userId1, Guid userId2);
    }
}
