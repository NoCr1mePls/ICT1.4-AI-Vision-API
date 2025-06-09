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


        //insert a new record
        [HttpPost("manual", Name = "ManualUpload" )]
        public async Task<ActionResult> AddManual(Litter litter)
        {
            litter.litter_id = Guid.NewGuid();
            litter.weather.weather_id = Guid.NewGuid();
            litter.weather_id = litter.weather.weather_id; ;

            var createdWeatherForecast = await _litterRepository.InsertAsync(litter, litter.weather);
            return Created();
        }

        [HttpPost("auto", Name = "AutoUpload")]
        public async Task<ActionResult> AddAuto(Litter litter)
        {
            // Fetch weather info
            string url = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={litter.location_latitude},{litter.location_longitude}";


            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error: " + response.StatusCode);
                Console.WriteLine("Request to weather API has failed");
                return StatusCode((int)response.StatusCode, "Weather API failed.");
            }

            string content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(content);

            // Prepare new weather + litter records
            litter.litter_id = Guid.NewGuid();
            litter.weather_id = Guid.NewGuid();

            if (litter.weather == null)
                litter.weather = new Weather(); // Ensure it's not null

            litter.weather.weather_id = (Guid)litter.weather_id;
            litter.weather.temperature_celsius = data.current?.temp_c;
            litter.weather.humidity = data.current?.humidity;
            litter.weather.conditions = data.current?.condition?.text;

            // Insert
            var createdWeatherForecast = await _litterRepository.InsertAsync(litter, litter.weather);

            return CreatedAtRoute("AutoUpload", new { id = litter.litter_id }, litter);
        }

    }
}
