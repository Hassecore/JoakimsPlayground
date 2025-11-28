namespace JoakimsPlaygroundFunctions.Triggers.ServiceBus
{
	public class ServiceBusMessage<T>
	{
		public Guid id { get; set; }
		public string? subject{ get; set; }
		public string? eventType { get; set; }
		public DateTime eventTime { get; set; }
		public string? dataVersion { get; set; }
		public string? metaDataVersion { get; set; }
		public T? data { get; set; }
	}
}
