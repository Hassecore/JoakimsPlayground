using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JoakimsPlaygroundFunctions.Triggers.HTTP;

public class HttpTrigger2
{
	private readonly ILogger<HttpTrigger2> _logger;

	public HttpTrigger2(ILogger<HttpTrigger2> logger)
	{
		_logger = logger;
	}

	[Function("Function2")]
	public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
	{
		string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

		var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnection");
		var queueName = Environment.GetEnvironmentVariable("QueueName");

		var client = new ServiceBusClient(connectionString, 
										  new ServiceBusClientOptions { TransportType = ServiceBusTransportType.AmqpWebSockets });

		var sender = client.CreateSender(queueName);

		var message = new ServiceBusMessage(requestBody);
		await sender.SendMessageAsync(message);

		_logger.LogInformation("C# HTTP trigger function processed a request 2.");
		return new OkObjectResult("Welcome to Azure Functions 2!");
	}
}