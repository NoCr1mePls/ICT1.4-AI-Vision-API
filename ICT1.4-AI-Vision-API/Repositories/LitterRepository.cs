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
        /// Adds a new Litter and its related Weather data to the database.
        /// </summary>
        /// <param name="litter">The Litter object to insert</param>
        /// <param name="weather">The correlated Weather object.</param>
        /// <returns>The inserted Litter + Weather model</returns>
        /// <exception cref="ArgumentException">Thrown when Litter ID and Weather ID do not match.</exception>
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

        /// <summary>
        /// Gets all Litter entries with + Weather data, sorted by detection time.
        /// </summary>
        /// <returns>List of all Litter entries</returns>
        public async Task<IEnumerable<Litter>> ReadAsync()
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a Litter entry by its ID
        /// </summary>
        /// <param name="id">The ID of the Litter to retrieve.</param>
        /// <returns>The Litter entry, or null if not found.<</returns>
        public async Task<Litter?> ReadAsync(Guid id)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .FirstOrDefaultAsync(l => l.litter_id == id);
        }

        /// <summary>
        /// Gets all Litter entries detected from a specific start time.
        /// </summary>
        /// <param name="startTime">The start time to filter from.</param>
        /// <returns>List of Litter entries.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .Where(l => l.detection_time >= startTime)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        /// <summary>
        /// Gets Litter entries from a specific time and classification.
        /// </summary>
        /// <param name="startTime">The start time to filter from</param>
        /// <param name="litterClassification">The litter classification to match.</param>
        /// <returns>List of Litter entries.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, int litterClassification)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .Where(l => l.detection_time >= startTime && l.litter_classification == litterClassification)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        /// <summary>
        /// Gets Litter entries between two times.
        /// </summary>
        /// <param name="startTime">The start time to filter from</param>
        /// <param name="stopTime">The stop time to filter from</param>
        /// <returns>List of Litter entries.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .Where(l => l.detection_time >= startTime && l.detection_time <= stopTime)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        /// <summary>
        /// Gets Litter entries between two times with a specific classification.
        /// </summary>
        /// <param name="startTime">The start time to filter from</param>
        /// <param name="stopTime">The stop time to filter from</param>
        /// <param name="litterClassification">The litter classification to match.</param>
        /// <returns>List of Litter entries.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime, int litterClassification)
        {
            return await _context.Litter
                .Include(l => l.Weather)
                .Where(l => l.detection_time >= startTime && l.detection_time <= stopTime && l.litter_classification == litterClassification)
                .OrderByDescending(l => l.detection_time)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all Litter entries with a specific classification.
        /// </summary>
        /// <param name="litterClassification">The litter classification to match.</param>
        /// <returns>List of Litter entries.</returns>
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
