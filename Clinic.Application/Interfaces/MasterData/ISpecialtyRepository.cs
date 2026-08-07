using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ISpecialtyRepository
    {
        Task<IEnumerable<Specialty>> GetAllAsync();
        Task<IEnumerable<Specialty>> GetAllActiveAsync();
        Task<Specialty?> GetByIdAsync(Guid id);
        Task<Specialty?> GetByCodeAsync(string code);
        Task AddAsync(Specialty specialty);
        void Update(Specialty specialty);
    }
}