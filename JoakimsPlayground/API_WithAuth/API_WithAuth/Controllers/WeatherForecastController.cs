using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WithAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        [Authorize]
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

		[HttpPut(Name = "SomeName")]
		[Authorize(Roles = "Admin")]
		public IEnumerable<int> Put()
		{
            
			return new List<int> { 1,2,3}
			.ToArray();
		}

		[HttpPost(Name = "SomeOtherName")]
		[Authorize(Roles = "User")]
		public IEnumerable<int> Post()
		{
			return new List<int> { 1, 2, 3 }
			.ToArray();
		}

		[HttpDelete(Name = "SomeThirdName")]
		//[Authorize(Roles = "Admin")]
		public IEnumerable<int> Delete()
		{
			return new List<int> { 1, 2, 3 }
			.ToArray();
		}
	}
}
