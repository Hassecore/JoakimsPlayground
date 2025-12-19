namespace Hassecore.API.Business.Managers
{
    public interface IUserPairingService
    {
        //Task<bool> IsAnyOfUsersPaired(Guid userId1, Guid userId2);
        Task<bool> CreateUserPairingRequestAsync(Guid senderId, Guid receiverId);
        Task<bool> AcceptUserPairingRequestAsync(Guid acceptingUserId, Guid userPairingRequestId);
        Task<bool> DenyUserPairingRequestAsync(Guid denyingUserId, Guid userPairingRequestId);
    }
}
