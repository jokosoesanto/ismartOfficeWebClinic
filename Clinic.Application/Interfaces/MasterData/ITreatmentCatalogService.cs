using Clinic.Application.DTOs.MasterData;
using Clinic.Domain.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ITreatmentCatalogService
    {
        Task<IEnumerable<TreatmentCatalogDto>> GetAllAsync();
        Task<IEnumerable<TreatmentCatalogDto>> GetBySubCategoryIdAsync(Guid subCategoryId);
        Task<TreatmentCatalogDto?> GetByIdAsync(Guid id);
        Task<TreatmentCatalog> CreateAsync(TreatmentCatalogCreateDto dto, Guid userId);
        Task<TreatmentCatalog> UpdateAsync(TreatmentCatalogUpdateDto dto, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
