using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IDiagnosisMasterService
    {
        Task<IEnumerable<DiagnosisMasterDto>> GetAllAsync();
        Task<DiagnosisMasterDto?> GetByIdAsync(Guid id);
        Task<DiagnosisMasterDto> CreateAsync(DiagnosisMasterCreateDto dto, Guid? createdBy = null);
        Task UpdateAsync(Guid id, DiagnosisMasterCreateDto dto, Guid? updatedBy = null);
        Task DeleteAsync(Guid id, Guid? deletedBy = null);
        Task<bool> ToggleActiveStatusAsync(Guid id, Guid? updatedBy = null);
        Task<bool> ExistsAsync(Guid id);
    }
}
