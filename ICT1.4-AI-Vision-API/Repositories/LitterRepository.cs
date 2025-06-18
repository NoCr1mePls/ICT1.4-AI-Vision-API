using SensoringApi.Data;
using SensoringApi.Interfaces;
using SensoringApi.Models;
using Microsoft.EntityFrameworkCore;

namespace SensoringApi.Repositories
{
    public class LitterRepository : ILitterRepository
    {
        private readonly LitterDbContext _context;

        public LitterRepository(LitterDbContext context)
        {
            _context = context;
        }

        public async Task<Litter> InsertAsync(Litter litter, Weather weather)
        {
            try
            {
                if (litter == null || weather == null)
                    throw new ArgumentNullException("Litter of Weather mag niet null zijn.");

                if (litter.litter_id != weather.weather_id)
                    throw new ArgumentException("De IDs van Litter en Weather moeten gelijk zijn.");

                litter.weather = weather;

                _context.Litter.Add(litter);
                _context.Weather.Add(weather);

                await _context.SaveChangesAsync();

                return await _context.Litter
                    .Include(l => l.weather)
                    .FirstOrDefaultAsync(l => l.litter_id == litter.litter_id)
                    ?? throw new InvalidOperationException("Litter niet gevonden na invoegen.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij InsertAsync voor Litter ID {litter.litter_id}", ex);
            }
        }

        public async Task<Litter?> ReadAsyncID(Guid id)
        {
            try
            {
                return await _context.Litter
                    .Include(l => l.weather)
                    .FirstOrDefaultAsync(l => l.litter_id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij ophalen van Litter met ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Litter>> ReadAsyncRange(DateTime? startTime, DateTime? stopTime, int? litterClassification)
        {
            try
            {
                var query = _context.Litter
                    .Include(l => l.weather)
                    .AsQueryable();

                if (startTime.HasValue)
                {
                    query = query.Where(l => l.detection_time >= startTime.Value);
                }

                if (stopTime.HasValue)
                {
                    query = query.Where(l => l.detection_time <= stopTime.Value);
                }

                if (litterClassification.HasValue)
                {
                    query = query.Where(l => l.litter_classification == litterClassification.Value);
                }

                return await query
                    .OrderByDescending(l => l.detection_time)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving Litter records with the specified filters.", ex);
            }
        }

    }
}
