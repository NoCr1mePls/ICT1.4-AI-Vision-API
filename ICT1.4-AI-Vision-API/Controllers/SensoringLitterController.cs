using ICT1._4_AI_Vision_API.Models;
using ICT1._4_AI_Vision_API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace ICT1._4_AI_Vision_API.Controllers
{
    [ApiController]
    [Route("litter")]
    public class SensoringLitterController : ControllerBase
    {
        private readonly ILitterRepository _litterRepository;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<SensoringLitterController> _logger;

        public SensoringLitterController(ILitterRepository litterRepository, ILogger<SensoringLitterController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _litterRepository = litterRepository;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["WeatherApiKey"]; // Verwijst naar user secrets of appsettings.json
        }

        /// <summary>
        /// Inserts a weather and litter record into the database after enriching the data with an external API.
        /// </summary>
        /// <param name="litter">The JSON litter model that was filled in</param>
        /// <returns>HTTP status codes + JSON litter model</returns>
        [HttpPost]
        public async Task<ActionResult> Add(Litter litter)
        {
            bool IsInvalid(double? value) => value == null || value == 0;

            if (IsInvalid(litter.location_latitude) && IsInvalid(litter.location_longitude))
            {
                return BadRequest(new { error = "Both latitude and longitude are missing or zero." });
            }
            else if (IsInvalid(litter.location_latitude))
            {
                return BadRequest(new { error = "Latitude is missing or zero." });
            }
            else if (IsInvalid(litter.location_longitude))
            {
                return BadRequest(new { error = "Longitude is missing or zero." });
            }

            string url = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={litter.location_latitude},{litter.location_longitude}";

            // Maak request naar de API
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new { error = "Weather API failed." });
            }

            // Lees response en parse JSON
            string content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(content);

            litter.litter_id = Guid.NewGuid();

            if (litter.Weather == null)
            {
                litter.Weather = new Weather();
            }

            // Vul weather model data in
            litter.Weather.weather_id = litter.litter_id;
            litter.Weather.temperature_celsius = data.current?.temp_c;
            litter.Weather.humidity = data.current?.humidity;
            litter.Weather.conditions = data.current?.condition?.text;

            // Verstuur naar database via de repository
            var createdRecord = await _litterRepository.InsertAsync(litter, litter.Weather);

            // Return status en model
            return CreatedAtAction(nameof(Get), new { id = litter.litter_id }, litter);
        }

        /// <summary>
        /// Finds a specific record by id in the database
        /// </summary>
        /// <param name="id">The unique identifier of the litter record.</param>
        /// <returns>HTTP status codes + JSON litter model</returns>
        [HttpGet("{id}", Name = "id")]
        public async Task<ActionResult<Litter>> Get(Guid id)
        {
            var litter = await _litterRepository.ReadAsync(id);
            if (litter == null)
            {
                return NotFound();
            }
            return Ok(litter);
        }
    }
}
