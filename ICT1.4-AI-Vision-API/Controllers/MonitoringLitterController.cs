using SensoringApi.Models;
using SensoringApi.Interfaces;
using SensoringApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SensoringApi.Classes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SensoringApi.Controllers
{
    [ApiController]
    [Route("litter")]
    public class MonitoringLitterController : ControllerBase
    {
        private readonly ILitterRepository _litterRepository;
        private readonly ILogger<MonitoringLitterController> _logger;
        MaxRange test = new MaxRange();

        public MonitoringLitterController(ILitterRepository litterRepository, ILogger<MonitoringLitterController> logger)
        {
            _litterRepository = litterRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves litter records detected on a specific day, optionally filtered.
        /// If no date is provided, today's date is used by default.
        /// </summary>
        /// <param name="date">The optional date to filter litter records.</param>
        /// <param name="classification">The optional litter classification to filter the records by.</param>
        /// <returns>https status codes + a filtered list of litter records</returns>
        [HttpGet("today", Name = "today")]
        public async Task<IActionResult> GetSingleDateData([FromQuery] DateOnly? date, [FromQuery] int? classification)
        {
            try
            {
                DateTime? filterDateStart = null;
                DateTime? filterDateEnd = null;
                int? filterClassification = null;
                if(date.HasValue)
                {
                    date = date.Value;
                }
                else
                {
                    date = DateOnly.FromDateTime(DateTime.Now);
                }
                if (classification.HasValue)
                {
                    filterClassification = classification.Value;
                }
                                
                DateTime startDateTime = date.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDateTime = date.Value.ToDateTime(TimeOnly.MaxValue);

                var data = await _litterRepository.ReadAsync(filterDateStart, filterDateEnd, classification.Value);
                if(data == null)
                {
                    return Ok(new { message = $"No Litter spotted for {startDateTime.ToString("yyyy-MM-dd")}" });
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij ophalen van gegevens.");
                return StatusCode(500, new { Message = "Er is een fout opgetreden bij het ophalen van de gegevens." });
            }
        }

        /// <summary>
        /// Retrieves litter records detected on a specific day, optionally filtered by classification.
        /// </summary>
        /// <param name="beginDate">The optional start date to filter records by.</param>
        /// <param name="endDate">The optional end date to filter records to by.</param>
        /// <param name="classification">The optional litter classification to filter the records by.</param>
        [HttpGet]
        public async Task<IActionResult> GetRangeDateData([FromQuery] DateOnly? beginDate, [FromQuery] DateOnly? endDate, [FromQuery] int? classification)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij ophalen van gegevens.");
                return StatusCode(500, new { Message = "Er is een fout opgetreden bij het ophalen van de gegevens." });
            }
        }

        /// <summary>
        /// Retrieves a list of predefined waste material categories.
        /// </summary>
        /// <returns>a list</returns>
        [HttpGet("Categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                List<string> categories = ["plastic (0/5)", "contaminated (1)", "glass (2)", "metal (3)", "paper (4)"];
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij ophalen van categorieën.");
                return StatusCode(500, new { Message = "Er is een fout opgetreden bij het ophalen van de categorieën." });
            }
        }
    }
}
