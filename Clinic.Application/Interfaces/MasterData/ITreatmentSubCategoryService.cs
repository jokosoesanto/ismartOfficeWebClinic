using Clinic.Application.DTOs.MasterData;
using Clinic.Domain.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ITreatmentSubCategoryService
    {
        Task<IEnumerable<TreatmentSubCategoryDto>> GetAllAsync();
        Task<IEnumerable<TreatmentSubCategoryDto>> GetByCategoryIdAsync(Guid categoryId);
        Task<TreatmentSubCategoryDto?> GetByIdAsync(Guid id);
        Task<TreatmentSubCategory> CreateAsync(TreatmentSubCategoryCreateDto dto, Guid userId);
        Task<TreatmentSubCategory> UpdateAsync(TreatmentSubCategoryUpdateDto dto, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
