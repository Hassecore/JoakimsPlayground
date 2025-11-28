using AuthServer.Models.Enums;

namespace AuthServer.Models
{
    public class ExternalUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ExternalUserId { get; set; }
        public EnumIdentityProviders Provider { get; set; }
    }
}
