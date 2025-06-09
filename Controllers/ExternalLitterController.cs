using HomeTry.Models;
using HomeTry.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HomeTry.Controllers
{
    [ApiController]
    [Route("litter")]
    public class ExternalLitterController : ControllerBase
    {
        private readonly LitterRepository _litterRepository;
        private readonly ILogger<ExternalLitterController> _logger;

        public ExternalLitterController(LitterRepository litterRepository, ILogger<ExternalLitterController> logger)
        {
            _litterRepository = litterRepository;
            _logger = logger;
        }


        //insert a new record
        [HttpPost]
        public async Task<ActionResult> Add(Litter litter)
        {
            var weather = new Weather
            {
                weather_id = Guid.NewGuid(),
            };
            litter.litter_id = Guid.NewGuid();

            var createdWeatherForecast = await _litterRepository.InsertAsync(litter, weather);
            return Created();
        }


        [HttpGet]
        //read all litter + weather records between certain dates
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


        //read a litter + weather record by litter id
        [HttpGet("{id}", Name = "id")]
        public async Task<ActionResult<Litter>> Get(Guid id)
        {
            var litter = await _litterRepository.ReadAsync(id);
            return Ok(litter);
        }
    }
}
