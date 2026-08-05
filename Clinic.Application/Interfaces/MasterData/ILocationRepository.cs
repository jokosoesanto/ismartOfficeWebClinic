using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(Guid id);
        Task<Location?> GetByCodeAsync(string code);
        Task<IEnumerable<Location>> GetAllAsync();
        Task AddAsync(Location location);
        Task UpdateAsync(Location location);
        Task<bool> HasChairsAsync(Guid id);
    }
}
