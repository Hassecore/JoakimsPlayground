namespace Hassecore.API.Data.Entities
{
    public class UserPairingRequest : IEntityBase
    {
        public required Guid Id { get; set; }
        public required Guid SenderId { get; set; }
        public User? Sender { get; set; }
        public required Guid ReceiverId { get; set; }
        public User? Receiver { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
