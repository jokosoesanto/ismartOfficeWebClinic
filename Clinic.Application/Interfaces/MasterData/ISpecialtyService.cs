using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ISpecialtyService
    {
        Task<IEnumerable<Specialty>> GetAllAsync();
        Task<IEnumerable<Specialty>> GetAllActiveAsync();
        Task<Specialty?> GetByIdAsync(Guid id);
        Task CreateAsync(Specialty specialty);
        Task UpdateAsync(Specialty specialty);
        Task DeleteAsync(Guid id, Guid deletedBy);
    }
}