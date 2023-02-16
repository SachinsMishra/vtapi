using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    public static class Extensions
    {
        public static StringContent AsJson(this object o)
            => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
    }
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        string apiURL = "https://sandbox-api.va.gov/services/veteran_confirmation/v0/status";
        string apiKey = "ateBQUYNtlzcBNQNyQ2MvbIuIPYKUZcn";
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("getWeather")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("getVTInformation")]
        public async Task<ObjectResult> getVTInformation([FromQuery] VTVerification verification)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apiKey", apiKey);
            var data = new
            {
                ssn = verification.SSN,
                first_name = verification.FirstName,
                last_name = verification.LastName,
                birth_date = verification.BirthDate,
                middle_name = verification.MiddleName,
                gender = verification.Gender
            };
            var result = await httpClient.PostAsync(apiURL, data.AsJson());
            if (result.IsSuccessStatusCode)
            {
                return Ok(result.Content.ReadAsStringAsync().Result);
            }
            else
            {
                return BadRequest(result.Content.ReadAsStringAsync().Result);
            }

        }
    }
}
