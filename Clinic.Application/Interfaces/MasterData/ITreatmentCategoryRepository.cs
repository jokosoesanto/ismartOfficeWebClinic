using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ITreatmentCategoryRepository
    {
        Task<TreatmentCategory?> GetByIdAsync(Guid id);
        Task<IEnumerable<TreatmentCategory>> GetAllAsync();
        Task<IEnumerable<TreatmentCategory>> GetActiveCategoriesAsync();
        Task AddAsync(TreatmentCategory category);
        Task UpdateAsync(TreatmentCategory category);
        Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null);
    }
}
