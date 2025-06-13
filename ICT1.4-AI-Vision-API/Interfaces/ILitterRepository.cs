using ICT1._4_AI_Vision_API.Models;

namespace ICT1._4_AI_Vision_API.Interfaces;

public interface ILitterRepository
{
     Task<Litter> InsertAsync(Litter litter, Weather weather);

     Task<IEnumerable<Litter>> ReadAsync();

     Task<Litter?> ReadAsync(Guid id);

     Task<IEnumerable<Litter>> ReadAsync(DateTime startTime);

     Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, int litterClassification);

     Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime);

     Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime, int litterClassification);

     Task<IEnumerable<Litter>> ReadAsync(int litterClassification);

}
