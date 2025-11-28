namespace AuthServer.Models
{
	public class RegisterDto
	{
		public string UserName { get; set; }
		public string Password{ get; set; }
		public string ConfirmPassword{ get; set; }
		public string Email{ get; set; }
	}
}
