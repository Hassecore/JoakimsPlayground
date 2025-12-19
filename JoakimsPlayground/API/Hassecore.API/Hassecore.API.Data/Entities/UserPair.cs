namespace Hassecore.API.Data.Entities
{
    public class UserPair : IEntityBase
    {
        public required Guid Id { get; set; }
        public required Guid User1Id { get; set; }
        public User? User1 { get; set; }

        public required Guid User2Id { get; set; }
        public User? User2 { get; set; }

        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
