// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.Messaging.ServiceBus;
using JoakimsPlaygroundFunctions.Business.DTOs;
using JoakimsPlaygroundFunctions.Business.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;

namespace JoakimsPlaygroundFunctions.Triggers.EventGrid;

public class EventGridTrigger2
{
	private readonly ILetterManager _letterManager;

	private readonly ILogger<EventGridTrigger2> _logger;

	public EventGridTrigger2(ILetterManager letterManager, ILogger<EventGridTrigger2> logger)
    {
		_letterManager = letterManager;
        _logger = logger;
    }

    //[Function(nameof(EventGridTrigger2))]
    public async Task Run([EventGridTrigger] EventGridEvent incomingEvent)
    {
		// This trigger picks up an event and stores persists payload (a Letter).
		var message = incomingEvent.Data.ToObjectFromJson<CreateLetterDto>();

		await _letterManager.CreateAsync(message, "EventGridTrigger");
	}
}