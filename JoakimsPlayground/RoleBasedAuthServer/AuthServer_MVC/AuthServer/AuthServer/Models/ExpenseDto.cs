using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models
{
	public class ExpenseDto
	{
		public int Id { get; set; }
		public decimal Value { get; set; }

		[Required]
		public string? Description { get; set; }
		//public string? Password { get; set; }
	}
}
