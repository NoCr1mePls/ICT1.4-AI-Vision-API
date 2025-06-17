using SensoringApi.Models;

namespace SensoringApi.Interfaces;

public interface ILitterRepository
{
     Task<Litter> InsertAsync(Litter litter, Weather weather);
     Task<Litter?> ReadAsync(Guid id);
     Task<IEnumerable<Litter>> ReadAsync(DateTime? startTime, DateTime? stopTime, int? litterClassification);

}
