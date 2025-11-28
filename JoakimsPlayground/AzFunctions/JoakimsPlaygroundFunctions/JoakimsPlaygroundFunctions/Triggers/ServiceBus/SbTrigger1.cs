using JoakimsPlaygroundFunctions.Business.DTOs;
using JoakimsPlaygroundFunctions.Business.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JoakimsPlaygroundFunctions.Triggers.ServiceBus;

public class SbTrigger1
{
	private readonly ILetterManager _letterManager;

	private readonly ILogger<SbTrigger1> _logger;

	public SbTrigger1(ILetterManager letterManager,
					  ILogger<SbTrigger1> logger)
    {
		_letterManager = letterManager;
		_logger = logger;
    }

    [Function(nameof(SbTrigger1))]
    public async Task Run(
        [ServiceBusTrigger("joakimsqueuename21", Connection = "ServiceBusConnection")]
		CreateLetterDto message)
    {
		await _letterManager.CreateAsync(message, "ServiceBusTrigger");
	}
}