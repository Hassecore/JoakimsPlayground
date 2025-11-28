using AuthServer.Models.Enums;

namespace AuthServer.Models
{
    public class IdentityProviderUserInfoDto
    {
        public EnumIdentityProviders Provider { get; set; }
        public string? UserSub { get; set; }
    }
}
