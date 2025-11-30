namespace Hassecore.API.Data.Entities
{
    public class User : IEntityBase
    {
        public required Guid Id { get; set; }
        public required string ExternalId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required DateOnly LastOnline { get; set; }
    }
}
