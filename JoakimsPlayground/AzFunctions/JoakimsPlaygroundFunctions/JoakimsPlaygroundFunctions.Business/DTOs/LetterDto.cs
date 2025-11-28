namespace JoakimsPlaygroundFunctions.Business.DTOs
{
	public class LetterDto
	{
		public Guid Id { get; set; }
		public string Sender { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public string? PS { get; set; }
	}
}
