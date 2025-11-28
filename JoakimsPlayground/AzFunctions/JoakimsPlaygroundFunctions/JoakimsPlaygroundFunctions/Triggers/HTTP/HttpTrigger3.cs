using Azure.Messaging.EventGrid;
using JoakimsPlaygroundFunctions.Business.DTOs;
using JoakimsPlaygroundFunctions.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace JoakimsPlaygroundFunctions.Triggers.HTTP;

public class HttpTrigger3
{
	private readonly ILogger<HttpTrigger3> _logger;
	private readonly ILetterManager _letterManager;

	public HttpTrigger3(ILetterManager letterManager,
						ILogger<HttpTrigger3> logger)
	{
		_letterManager = letterManager;
		_logger = logger;
	}

	[Function("HttpTrigger3")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
	{
		string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

		var handshakeResult = await Handshake(requestBody);

		if (handshakeResult != null) return handshakeResult;

		return await Handle(requestBody);
	}

	private async Task<OkResult> Handle(string requestBody)
	{
		var events = JsonSerializer.Deserialize<List<EventGridEvent>>(requestBody);

		foreach (var eventGridEvent in events)
		{
			// Handle your actual event payload here
			_logger.LogInformation("Event received: Subject={subject}, ID={id}",
				eventGridEvent.Subject, eventGridEvent.Id);


			var createLetterDto = JsonSerializer.Deserialize<CreateLetterDto>(eventGridEvent.Data.ToString());
			if(createLetterDto == null) continue;

			var id = await _letterManager.CreateAsync(createLetterDto, "HttpTrigger3");
			var letter = _letterManager.Get(id);

			// Example: process the event data
			_logger.LogInformation("Data: {data}", eventGridEvent.Data);
		}

		return new OkResult();
	}

	private async Task<OkObjectResult?> Handshake(string requestBody)
	{
		var events = JsonSerializer.Deserialize<JsonElement>(requestBody);

		foreach (var ev in events.EnumerateArray())
		{
			var eventType = ev.GetProperty("eventType").GetString();

			if (eventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
			{
				var validationCode = ev.GetProperty("data").GetProperty("validationCode").GetString();
				return new OkObjectResult(new { validationResponse = validationCode });
			}
		}

		return null;
	}
}