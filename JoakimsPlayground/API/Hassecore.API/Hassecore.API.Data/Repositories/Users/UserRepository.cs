using Hassecore.API.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hassecore.API.Data.Repositories.Users
{
    public class UserRepository : BaseRepository, IUserRepository 
    {
        //private readonly HassecoreApiContext _context;

        public UserRepository(HassecoreApiContext context) : base(context)
        {
            //_context = context;
        }

        public bool RequestUserPairing(Guid userId1, Guid userId2)
        {



            throw new NotImplementedException();
        }
    }
}
