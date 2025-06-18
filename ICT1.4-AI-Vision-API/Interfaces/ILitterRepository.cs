using SensoringApi.Models;

namespace SensoringApi.Interfaces;

public interface ILitterRepository
{
     Task<Litter> InsertAsync(Litter litter, Weather weather);
     Task<Litter?> ReadAsyncID(Guid id);
     Task<IEnumerable<Litter>> ReadAsyncRange(DateTime? startTime, DateTime? stopTime, int? litterClassification);

}
