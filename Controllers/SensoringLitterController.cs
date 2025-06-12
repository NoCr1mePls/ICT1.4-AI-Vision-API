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
            _apiKey = configuration["WeatherApiKey"];//refers to the user secrets for the api key
        }

        /// <summary>
        /// Inserts a weather and litter record into the database after enriching the data with an external api.
        /// </summary>
        /// <param name="litter">the json litter model that was filled in</param>
        /// <returns>http status codes + json litter model</returns>
        [HttpPost]
        public async Task<ActionResult> Add(Litter litter)
        {
            string url = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={litter.location_latitude},{litter.location_longitude}";

            //checks if location is valid
            if (litter.location_latitude == null && litter.location_latitude == 0 && litter.location_longitude == null && litter.location_longitude == 0)
            {
                return BadRequest(new { error = "Both latitude and longitude are missing or zero." });
            }
            else if (litter.location_latitude == null && litter.location_latitude == 0)
            {
                return BadRequest(new { error = "Latitude is missing or zero." });
            }
            else if (litter.location_longitude == null && litter.location_longitude == 0)
            {
                return BadRequest(new { error = "Longitude is missing or zero." });
            }

            //make request to api
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new { error = "Weather API failed." });
            }

            //read info and turn it into usable variables
            string content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(content);

            litter.litter_id = Guid.NewGuid();

            if (litter.weather == null)
            {
                litter.weather = new Weather();
            }

            //fill in weather model data
            litter.weather.weather_id = (Guid)litter.litter_id;
            litter.weather.temperature_celsius = data.current?.temp_c;
            litter.weather.humidity = data.current?.humidity;
            litter.weather.conditions = data.current?.condition?.text;

            //send to database
            var createdWeatherForecast = await _litterRepository.InsertAsync(litter, litter.weather);

            //return status code and model
            return CreatedAtAction(nameof(Get), new { id = litter.litter_id }, litter);;
        }

        /// <summary>
        /// Finds a specific record by id in the database
        /// </summary>
        /// <param name="id">The unique identifier of the litter record.</param>
        /// <returns>http status codes + json litter model</returns>
        [HttpGet("{id}", Name = "id")]
        public async Task<ActionResult<Litter>> Get(Guid id)
        {
            var litter = await _litterRepository.ReadAsync(id);
            return Ok(litter);
        }

    }
}
