// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace JoakimsPlaygroundFunctions.Triggers.EventGrid;

public class EventGridTrigger1
{
    private readonly ILogger<EventGridTrigger1> _logger;

    public EventGridTrigger1(ILogger<EventGridTrigger1> logger)
    {
        _logger = logger;
    }

    //[Function(nameof(EventGridTrigger1))]
    public async Task Run([EventGridTrigger] EventGridEvent incomingEvent)
    {
        // This trigger picks up an event and puts it on a queue.
        var messageString = incomingEvent.Data.ToString();
        var messageString2 = incomingEvent.Data.ToObjectFromJson<CustomEventData>();


		var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnection");
		var queueName = Environment.GetEnvironmentVariable("QueueName");

		var client = new ServiceBusClient(connectionString,
										  new ServiceBusClientOptions { TransportType = ServiceBusTransportType.AmqpWebSockets });

		var sender = client.CreateSender(queueName);

        var message = new ServiceBusMessage(messageString);
		await sender.SendMessageAsync(message);

    }
}