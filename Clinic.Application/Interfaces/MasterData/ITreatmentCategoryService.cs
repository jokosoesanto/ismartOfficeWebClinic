using Clinic.Domain.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ITreatmentCategoryService
    {
        Task<IEnumerable<TreatmentCategory>> GetAllAsync();
        Task<TreatmentCategory?> GetByIdAsync(Guid id);
        Task<TreatmentCategory> CreateAsync(TreatmentCategory category, Guid userId);
        Task<TreatmentCategory> UpdateAsync(Clinic.Application.DTOs.MasterData.TreatmentCategoryUpdateDto category, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
        Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null);
    }
}
