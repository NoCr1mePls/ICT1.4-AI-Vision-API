using HomeTry.Models;
using HomeTry.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace HomeTry.Controllers
{
    [ApiController]
    [Route("litter")]
    public class MonitoringLitterController : ControllerBase
    {
        private readonly LitterRepository _litterRepository;
        private readonly ILogger<MonitoringLitterController> _logger;

        public MonitoringLitterController(LitterRepository litterRepository, ILogger<MonitoringLitterController> logger)
        {
            _litterRepository = litterRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves litter records detected on a specific day, optionally filtered.
        /// If no date is provided, today's date is used by default.
        /// </summary>
        /// <param name="date">The optional date to filter litter records.</param>
        /// <param name="cat">The optional litter classification to filter the records by.</param>
        /// <returns>https status codes + a filtered list of litter records</returns>
        [HttpGet("today", Name = "today")]
        public async Task<IActionResult> Get([FromQuery] DateOnly? date, [FromQuery] int? cat)
        {
            if (date.HasValue && cat.HasValue)
            {
                DateTime startDateTime = date.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDateTime = date.Value.ToDateTime(TimeOnly.MaxValue);

                var data = await _litterRepository.ReadAsync(startDateTime, endDateTime, cat.Value);
                return Ok(data);
            }
            if (date.HasValue)
            {
                DateTime startDateTime = date.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDateTime = date.Value.ToDateTime(TimeOnly.MaxValue);

                var data = await _litterRepository.ReadAsync(startDateTime, endDateTime);
                return Ok(data);
            }
            if (cat.HasValue)
            {
                date = DateOnly.FromDateTime(DateTime.Now);

                DateTime startDateTime = date.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDateTime = date.Value.ToDateTime(TimeOnly.MaxValue);

                var data = await _litterRepository.ReadAsync(startDateTime, endDateTime, cat.Value);
                return Ok(data);
            }
            else
            {
                date = DateOnly.FromDateTime(DateTime.Now);
                DateTime startDateTime = date.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDateTime = date.Value.ToDateTime(TimeOnly.MaxValue);

                var data = await _litterRepository.ReadAsync(startDateTime, endDateTime);
                return Ok(data);
            }
        }

        /// <summary>
        /// Retrieves litter records detected on a specific day, optionally filtered by classification.
        /// </summary>
        /// <param name="beginDate">The optional start date to filter records by.</param>
        /// <param name="endDate">The optional end date to filter records to by.</param>
        /// <param name="cat">The optional litter classification to filter the records by.</param>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateOnly? beginDate, [FromQuery] DateOnly? endDate, [FromQuery] int? cat)
        {
            if (beginDate.HasValue && endDate.HasValue && cat.HasValue)
            {
                DateTime startDateTime = beginDate.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDateTime = endDate.Value.ToDateTime(TimeOnly.MaxValue);

                var data = await _litterRepository.ReadAsync(startDateTime, endDateTime, cat.Value);
                return Ok(data);
            }
            if (beginDate.HasValue && endDate.HasValue)
            {
                DateTime startDateTime = beginDate.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDateTime = endDate.Value.ToDateTime(TimeOnly.MaxValue);

                var data = await _litterRepository.ReadAsync(startDateTime, endDateTime);
                return Ok(data);
            }
            if (beginDate.HasValue && cat.HasValue)
            {
                DateTime startDateTime = beginDate.Value.ToDateTime(TimeOnly.MinValue);

                var data = await _litterRepository.ReadAsync(startDateTime, cat.Value);
                return Ok(data);
            }
            if (beginDate.HasValue)
            {
                DateTime startDateTime = beginDate.Value.ToDateTime(TimeOnly.MinValue);

                var data = await _litterRepository.ReadAsync(startDateTime);
                return Ok(data);
            }
            if (cat.HasValue)
            {
                var data = await _litterRepository.ReadAsync(cat.Value);
                return Ok(data);
            }
            else
            {
                var data = await _litterRepository.ReadAsync();
                return Ok(data);
            }
        }

        [HttpGet]
        public async Task<IEnumerable> GetCategories()
        {
            List<string> categories = ["paper", "plastic", "biodegradable", "cardboard", "glass", "metal"];

            return categories;
        }
    }
}
