using JoakimsPlaygroundFunctions.Business.DTOs;
using JoakimsPlaygroundFunctions.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace JoakimsPlaygroundFunctions.Triggers.HTTP;

public class HttpTrigger1
{
    private readonly ILogger<HttpTrigger1> _logger;
   private readonly ILetterManager _letterManager;

    public HttpTrigger1(ILetterManager letterManager,
                        ILogger<HttpTrigger1> logger)
    {
		_letterManager = letterManager;
        _logger = logger;
    }

    [Function("Function1")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
		var handshakeResult = await Handshake(req);

		if (handshakeResult != null) return handshakeResult;

		return await Handle(req);
    }

    private async Task<OkObjectResult> Handle(HttpRequest req)
    {
		CreateLetterDto dto = new CreateLetterDto
		{
			Sender = "Joakim",
			Content = $"HttpTriggeredLetter at {DateTime.UtcNow}",
			PS = "PS from Azure Functions!"
		};

		var id = await _letterManager.CreateAsync(dto, "HttpTrigger");
		var letter = _letterManager.Get(id);

		return new OkObjectResult(letter);
	}


	private async Task<OkObjectResult?> Handshake(HttpRequest req)
    {
		string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
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