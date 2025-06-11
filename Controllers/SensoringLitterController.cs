using HomeTry.Models;
using HomeTry.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text.Json;

namespace HomeTry.Controllers
{
    [ApiController]
    [Route("litter")]
    public class SensoringLitterController : ControllerBase
    {
        private readonly LitterRepository _litterRepository;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<SensoringLitterController> _logger;

        public SensoringLitterController(LitterRepository litterRepository, ILogger<SensoringLitterController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _litterRepository = litterRepository;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["WeatherApiKey"];
        }

        [HttpPost]
        public async Task<ActionResult> Add(Litter litter)
        {
            // Fetch weather info
            string url = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={litter.location_latitude},{litter.location_longitude}";


            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new { error = "Weather API failed." });

            }

            string content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(content);

            litter.litter_id = Guid.NewGuid();

            if (litter.weather == null)
            {
                litter.weather = new Weather();
            }

            litter.weather.weather_id = (Guid)litter.litter_id;
            litter.weather.temperature_celsius = data.current?.temp_c;
            litter.weather.humidity = data.current?.humidity;
            litter.weather.conditions = data.current?.condition?.text;

            var createdWeatherForecast = await _litterRepository.InsertAsync(litter, litter.weather);

            return CreatedAtAction(nameof(Add), new { id = litter.litter_id }, litter);
        }

        [HttpGet("{id}", Name = "id")]
        public async Task<ActionResult<Litter>> Get(Guid id)
        {
            var litter = await _litterRepository.ReadAsync(id);
            return Ok(litter);
        }

    }
}
