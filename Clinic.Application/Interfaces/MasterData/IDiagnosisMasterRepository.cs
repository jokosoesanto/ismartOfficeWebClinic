using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IDiagnosisMasterRepository
    {
        Task<IEnumerable<DiagnosisMaster>> GetAllAsync();
        Task<DiagnosisMaster?> GetByIdAsync(Guid id);
        Task<DiagnosisMaster?> GetByCodeAsync(string code);
        Task<DiagnosisMaster> AddAsync(DiagnosisMaster diagnosisMaster);
        Task UpdateAsync(DiagnosisMaster diagnosisMaster);
        Task DeleteAsync(Guid id, Guid? deletedBy = null);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
    }
}
