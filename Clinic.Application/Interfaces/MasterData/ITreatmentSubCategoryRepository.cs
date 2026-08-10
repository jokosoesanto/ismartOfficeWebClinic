using Clinic.Domain.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ITreatmentSubCategoryRepository
    {
        Task<TreatmentSubCategory?> GetByIdAsync(Guid id);
        Task<IEnumerable<TreatmentSubCategory>> GetAllAsync();
        Task<IEnumerable<TreatmentSubCategory>> GetByCategoryIdAsync(Guid categoryId);
        Task AddAsync(TreatmentSubCategory subCategory);
        Task UpdateAsync(TreatmentSubCategory subCategory);
        Task<bool> IsNameUniqueAsync(Guid categoryId, string name, Guid? excludeId = null);
    }
}
