namespace AuthServer.Models
{
	public class TokenEndpointDto
	{
		public string AuthorizationCode { get; set; }
		public int ClientId { get; set; } // not implemented to be used yet
		public string CodeVerifier { get; set; }
		public string GrantType { get; set; } // not implemented to be used yet
	}
}
