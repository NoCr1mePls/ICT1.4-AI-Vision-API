﻿using SensoringApi.Models;
using SensoringApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace SensoringApi.Controllers
{
    [ApiController]
    [Route("litter")]
    public class SensoringLitterController : ControllerBase
    {
        private readonly ILitterRepository _litterRepository;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<SensoringLitterController> _logger;
        private readonly string _accesToken;

        public SensoringLitterController(
            ILitterRepository litterRepository,
            ILogger<SensoringLitterController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _litterRepository = litterRepository;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration.GetValue<string>("WeatherApiKey"); // Verwijst naar user secrets of appsettings.json
            _accesToken = configuration.GetValue<string>("SensorAccesToken");
        }

        /// <summary>
        /// Inserts a weather and litter record into the database after enriching the data with an external API.
        /// </summary>
        /// <param name="litter">The JSON litter model that was filled in</param>
        /// <returns>HTTP status codes + JSON litter model</returns>
        [HttpPost]
        public async Task<ActionResult> Add([FromBody]Litter litter, [FromHeader]string token)
        {
            if (token == _accesToken)
            {
                try
                {
                    if ((litter.location_latitude == null || litter.location_latitude == 0) &&
                        (litter.location_longitude == null || litter.location_longitude == 0))
                    {
                        return BadRequest(new { error = "Zowel latitude als longitude ontbreken of zijn ongeldig." });
                    }
                    else if (litter.location_latitude == null || litter.location_latitude == 0)
                    {
                        return BadRequest(new { error = "Latitude ontbreekt of is ongeldig." });
                    }
                    else if (litter.location_longitude == null || litter.location_longitude == 0)
                    {
                        return BadRequest(new { error = "Longitude ontbreekt of is ongeldig." });
                    }

                    string url = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={litter.location_latitude},{litter.location_longitude}";

                    HttpResponseMessage response = await _httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Weather API call failed with status code {StatusCode}", response.StatusCode);
                        return StatusCode((int)response.StatusCode, new { error = "Weather API mislukt." });
                    }

                    string content = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(content);

                    litter.litter_id = Guid.NewGuid();
                    if (litter.detection_time == null)
                    {
                        litter.detection_time = DateTime.Now;
                    }

                    if (litter.weather == null)
                    {
                        litter.weather = new Weather();
                    }

                    litter.weather.weather_id = litter.litter_id;
                    litter.weather.temperature_celsius = data.current?.temp_c;
                    litter.weather.humidity = data.current?.humidity;
                    litter.weather.conditions = data.current?.condition?.text;

                    var createdRecord = await _litterRepository.InsertAsync(litter, litter.weather);

                    return Created();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fout bij toevoegen van litter record.");
                    return StatusCode(500,new { error = "Er is een interne serverfout opgetreden." });
                }
            }
            else return Unauthorized(new { error = "U mist authorisatie." });
        }

        /// <summary>
        /// Finds a specific record by id in the database
        /// </summary>
        /// <param name="id">The unique identifier of the litter record.</param>
        /// <returns>HTTP status codes + JSON litter model</returns>
        [HttpGet("{id}", Name = "id")]
        public async Task<ActionResult<Litter>> Get(Guid id)
        {
            try
            {
                var litter = await _litterRepository.ReadAsyncID(id);
                if (litter == null)
                {
                    return NotFound();
                }
                return Ok(litter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij ophalen van litter record met id {Id}.", id);
                return BadRequest(new { error = "Er is een interne serverfout opgetreden." });
            }
        }
    }
}
