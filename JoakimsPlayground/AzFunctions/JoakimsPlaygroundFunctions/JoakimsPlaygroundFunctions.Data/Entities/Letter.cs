namespace JoakimsPlaygroundFunctions.Data.Entities
{
	public class Letter : EntityBase
	{
		public string Sender { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public string? PS { get; set; } = string.Empty;
	}
}
