using Hassecore.API.Data.Entities;
using Hassecore.API.Data.Repositories;

namespace Hassecore.API.Business.Managers
{
    public class UserPairingService : IUserPairingService
    {
        IBaseRepository _baseRepository;
        public UserPairingService(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<bool> CreateUserPairingRequestAsync(Guid senderId, Guid receiverId)
        {
            var userPairingRequestExists = DoesUserPairingRequestExist(senderId, receiverId);
            if (IsAnyOfUsersPaired(senderId, receiverId) ||
                userPairingRequestExists)
            {
                return false;
            }

            var userPairingRequest = new UserPairingRequest
            {
                Id =  Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                CreatedAt = DateTime.UtcNow
            };
            
            await _baseRepository.CreateAsync(userPairingRequest);

            return true;
        }

        public async Task<bool> AcceptUserPairingRequestAsync(Guid acceptingUserId, Guid userPairingRequestId)
        {
            var pairingRequest = await _baseRepository.GetSingleOrDefaultAsync<UserPairingRequest>(x=> x.ReceiverId == acceptingUserId && 
                                                                                                       x.Id == userPairingRequestId);
            if (pairingRequest == null)
            {
                return false;
            }

            if (IsAnyOfUsersPaired(pairingRequest.SenderId, pairingRequest.ReceiverId))
            {
                return false;
            }

            var userPair = new UserPair
            {
                Id = Guid.NewGuid(),
                User1Id = pairingRequest.SenderId,
                User2Id = pairingRequest.ReceiverId,
                CreatedAt = DateTime.UtcNow
            };
            await _baseRepository.CreateAsync(userPair);

            await _baseRepository.DeleteAsync<UserPairingRequest>(pairingRequest.Id);

            return true;
        }

        public async Task<bool> DenyUserPairingRequestAsync(Guid denyingUserId, Guid userPairingRequestId)
        {
            var pairingRequest = await _baseRepository.GetSingleOrDefaultAsync<UserPairingRequest>(x => x.ReceiverId == denyingUserId && 
                                                                                                        x.Id == userPairingRequestId);
            if (pairingRequest == null)
            {
                return false;
            }
            await _baseRepository.DeleteAsync<UserPairingRequest>(pairingRequest.Id);
            return true;
        }

        private bool DoesUserPairingRequestExist(Guid userId1, Guid userId2) => 
            _baseRepository.GetQueryable<UserPairingRequest>(x => (x.SenderId == userId1 && x.ReceiverId == userId2) ||
                                                                  (x.SenderId == userId2 && x.ReceiverId == userId1))
                           .Any();

        private bool IsAnyOfUsersPaired(Guid userId1, Guid userId2)
        {
            var anyUserPair = _baseRepository.GetQueryable<UserPair>(up => (up.User1Id == userId1 || up.User2Id == userId1) ||
                                                                        (up.User1Id == userId2 || up.User2Id == userId2))
                                          .Any();

            return anyUserPair;
        }
    }
}
