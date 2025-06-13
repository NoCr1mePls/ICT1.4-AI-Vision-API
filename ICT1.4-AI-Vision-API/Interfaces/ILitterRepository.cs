using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICT1._4_AI_Vision_API.Models;

namespace ICT1._4_AI_Vision_API.Interfaces
{
    public interface ILitterRepository
    {
        Task<Litter> InsertAsync(Litter litter, Weather weather);
        Task<Litter?> ReadAsync(Guid id);

        Task<IEnumerable<Litter>> ReadAsync();
        Task<IEnumerable<Litter>> ReadAsync(DateTime startDateTime, DateTime endDateTime);
        Task<IEnumerable<Litter>> ReadAsync(DateTime startDateTime, DateTime endDateTime, int category);
        Task<IEnumerable<Litter>> ReadAsync(DateTime startDateTime, int category);
        Task<IEnumerable<Litter>> ReadAsync(DateTime startDateTime);
        Task<IEnumerable<Litter>> ReadAsync(int category);
    }
}
