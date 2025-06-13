using HomeTry.Data;
using HomeTry.Interfaces;
using HomeTry.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeTry.Repositories
{
    public class LitterRepository : ILitterRepository
    {
        private readonly LitterDbContext _context;

        public LitterRepository(LitterDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Inserts a litter record with its matching weather record (same Guid).
        /// </summary>
        public async Task<Litter> InsertAsync(Litter litter, Weather weather)
        {
            if (litter.litter_id != weather.weather_id)
                throw new ArgumentException("De IDs van Litter en Weather moeten gelijk zijn.");

            // Koppel Weather aan Litter
            litter.Weather = weather;

            // Voeg beide records toe
            _context.Litter.Add(litter);
            _context.Weather.Add(weather);

            await _context.SaveChangesAsync();

            return await _context.Litter
                .Include(l => l.Weather)
                .FirstOrDefaultAsync(l => l.litter_id == litter.litter_id);
        }

        public async Task<IEnumerable<Litter>> ReadAsync()
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        public async Task<Litter?> ReadAsync(Guid id)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .FirstOrDefaultAsync(l => l.litter_id == id);
        }

        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .Where(l => l.detection_time >= startTime)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, int litterClassification)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .Where(l => l.detection_time >= startTime && l.litter_classification == litterClassification)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .Where(l => l.detection_time >= startTime && l.detection_time <= stopTime)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime, int litterClassification)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .Where(l => l.detection_time >= startTime && l.detection_time <= stopTime && l.litter_classification == litterClassification)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        public async Task<IEnumerable<Litter>> ReadAsync(int litterClassification)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .Where(l => l.litter_classification == litterClassification)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }
    }
}
