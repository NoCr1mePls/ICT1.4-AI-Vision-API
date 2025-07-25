﻿using SensoringApi.Models;
using SensoringApi.Interfaces;
using SensoringApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SensoringApi.Classes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

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
                    if (!test.DateAllowed(null, date))
                    {
                        return BadRequest(new { error = $"{date.Value.ToString("yyyy-MM-dd")} not allowed, a date needs to be between 2025-05-01 and {DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")} in order to be valid" });
                    }
                    date = date.Value;
                    filterDateStart = date.Value.ToDateTime(TimeOnly.MinValue);
                    filterDateEnd = date.Value.ToDateTime(TimeOnly.MaxValue);
                }
                else
                {
                    date = DateOnly.FromDateTime(DateTime.Now);
                    filterDateStart = date.Value.ToDateTime(TimeOnly.MinValue);
                    filterDateEnd = date.Value.ToDateTime(TimeOnly.MaxValue);
                }
                if (classification.HasValue)
                {
                    filterClassification = classification.Value;
                }

                var data = await _litterRepository.ReadAsyncRange(filterDateStart, filterDateEnd, filterClassification);
                if (data == null || !data.Any())
                {
                    return Ok(new { message = $"No Litter spotted for {filterDateStart.Value.ToString("yyyy-MM-dd")}" });
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
                DateTime? filterDateStart = null;
                DateTime? filterDateEnd = null;
                int? filterClassification = null;
                if (beginDate.HasValue)
                {
                    if (!test.DateAllowed(null, beginDate))
                    {
                        return BadRequest(new { error = $"{beginDate.Value.ToString("yyyy-MM-dd")} not allowed, a date needs to be between 2025-05-01 and {DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")} in order to be valid" });
                    }
                    filterDateStart = beginDate.Value.ToDateTime(TimeOnly.MinValue);
                }
                if (endDate.HasValue)
                {
                    if (!test.DateAllowed(null, endDate))
                    {
                        return BadRequest(new { error = $"{endDate.Value.ToString("yyyy-MM-dd")} not allowed, a date needs to be between 2025-05-01 and {DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")} in order to be valid" });
                    }
                    filterDateEnd = endDate.Value.ToDateTime(TimeOnly.MaxValue);
                }
                if (classification.HasValue)
                {
                    filterClassification = classification.Value;
                }

                var data = await _litterRepository.ReadAsyncRange(filterDateStart, filterDateEnd, filterClassification);
                if (data == null || !data.Any())
                {
                    return Ok(new { message = $"No Litter spotted between {filterDateStart.Value.ToString("yyyy-MM-dd")} & {filterDateEnd.Value.ToString("yyyy-MM-dd")}" });
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
        /// Retrieves a list of predefined waste material categories.
        /// </summary>
        /// <returns>a list</returns>
        [HttpGet("Categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                List<string> categories = ["0 battery", "1 cardboard", "2 glass", "3 metal", "4 organic", "5 paper", "6 plastic", "7 tissue"];
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
