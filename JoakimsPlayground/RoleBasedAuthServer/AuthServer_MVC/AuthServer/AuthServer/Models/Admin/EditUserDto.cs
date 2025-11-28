namespace AuthServer.Models.Admin
{
	public class EditUserDto
	{
		public string? Id { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? NewPassword { get; set; }
		public string? ConfirmedNewPassword { get; set; }
	}
}
