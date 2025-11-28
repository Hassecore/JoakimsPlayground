namespace JoakimsPlaygroundFunctions.Business.DTOs
{
	public class CreateLetterDto
	{
		public string Sender { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public string? PS { get; set; }
	}
}
