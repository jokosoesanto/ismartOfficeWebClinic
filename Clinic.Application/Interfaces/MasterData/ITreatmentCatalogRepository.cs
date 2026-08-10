using Clinic.Domain.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ITreatmentCatalogRepository
    {
        Task<IEnumerable<TreatmentCatalog>> GetAllAsync();
        Task<IEnumerable<TreatmentCatalog>> GetBySubCategoryIdAsync(Guid subCategoryId);
        Task<TreatmentCatalog?> GetByIdAsync(Guid id);
        Task AddAsync(TreatmentCatalog entity);
        Task UpdateAsync(TreatmentCatalog entity);
        Task<bool> IsNameUniqueAsync(Guid subCategoryId, string name, Guid? excludeId = null);
    }
}
