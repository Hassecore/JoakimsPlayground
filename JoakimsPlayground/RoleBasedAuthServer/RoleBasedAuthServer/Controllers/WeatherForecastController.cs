using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoleBasedAuthServer.Controllers;

[ApiController]
[Route("[controller]")]
//[Authorize]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

	[HttpPut(Name = "SomeOtherName")]
	[Authorize(Roles = "Admin")]

	public IEnumerable<int> Get2()
	{
		return new List<int> { 1, 2, 3 }
		.ToArray();
	}

	[HttpPost(Name = "SomeOtherName")]
	[Authorize(Roles = "User")]
	public IEnumerable<int> Get3()
	{
		return new List<int> { 1, 2, 3 }
		.ToArray();
	}
}
