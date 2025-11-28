using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models.Admin
{
	public class UserOverviewDto
	{
		[Required]
		public string? Id { get; set; }
		[Required]
		public string? UserName { get; set; }
		[Required]
		public string? Email { get; set; }
	}
}
