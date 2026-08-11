using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IInsuranceService
    {
        Task<List<InsuranceListDto>> GetAllAsync();
        Task<InsuranceDto?> GetByIdAsync(Guid id);
        Task<InsuranceCreateEditDto?> GetForEditAsync(Guid id);
        Task<Guid> CreateAsync(InsuranceCreateEditDto dto);
        Task UpdateAsync(InsuranceCreateEditDto dto);
        Task DeleteAsync(Guid id);
    }
}
