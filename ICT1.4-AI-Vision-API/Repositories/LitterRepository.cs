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

        public async Task<IEnumerable<Litter>> ReadAsync()
        {
            try
            {
                return await _context.Litter
                    .Include(l => l.weather)
                    .OrderByDescending(l => l.detection_time)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij ophalen van alle Litter items.", ex);
            }
        }

        public async Task<Litter?> ReadAsync(Guid id)
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

        public async Task<IEnumerable<Litter>> ReadAsyncStart(DateTime startTime)
        {
            try
            {
                return await _context.Litter
                    .Include(l => l.weather)
                    .Where(l => l.detection_time >= startTime)
                    .OrderByDescending(l => l.detection_time)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij ophalen van Litter vanaf starttijd.", ex);
            }
        }

        public async Task<IEnumerable<Litter>> ReadAsyncStop(DateTime stopTime)
        {
            try
            {
                return await _context.Litter
                    .Include(l => l.weather)
                    .Where(l => l.detection_time <= stopTime)
                    .OrderByDescending(l => l.detection_time)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij ophalen van Litter tot stoptijd.", ex);
            }
        }

        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, int litterClassification)
        {
            try
            {
                return await _context.Litter
                    .Include(l => l.weather)
                    .Where(l => l.detection_time >= startTime &&
                                l.litter_classification == litterClassification)
                    .OrderByDescending(l => l.detection_time)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij ophalen van Litter op starttijd + classificatie.", ex);
            }
        }

        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime)
        {
            try
            {
                return await _context.Litter
                    .Include(l => l.weather)
                    .Where(l => l.detection_time >= startTime &&
                                l.detection_time <= stopTime)
                    .OrderByDescending(l => l.detection_time)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij ophalen van Litter tussen tijden.", ex);
            }
        }

        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime, int litterClassification)
        {
            try
            {
                return await _context.Litter
                    .Include(l => l.weather)
                    .Where(l => l.detection_time >= startTime &&
                                l.detection_time <= stopTime &&
                                l.litter_classification == litterClassification)
                    .OrderByDescending(l => l.detection_time)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij ophalen van Litter tussen tijden + classificatie.", ex);
            }
        }

        public async Task<IEnumerable<Litter>> ReadAsync(int litterClassification)
        {
            try
            {
                return await _context.Litter
                    .Include(l => l.weather)
                    .Where(l => l.litter_classification == litterClassification)
                    .OrderByDescending(l => l.detection_time)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij ophalen van Litter op classificatie.", ex);
            }
        }
    }
}
