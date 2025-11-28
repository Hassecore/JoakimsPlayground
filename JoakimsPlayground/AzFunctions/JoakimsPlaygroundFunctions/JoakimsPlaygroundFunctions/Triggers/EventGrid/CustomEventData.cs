using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoakimsPlaygroundFunctions.Triggers.EventGrid
{
	public class CustomEventData
	{
		public string? MessageId { get; set; }

		public string? From { get; set; }

		public string? To { get; set; }

		public string? Message { get; set; }

		public DateTime ReceivedTimestamp { get; set; }
	}
}
