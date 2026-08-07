using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Domain.Entities.System;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IMasterReferenceService
    {
        Task<IEnumerable<MasterReference>> GetByCategoryAsync(string category, bool activeOnly = true, CancellationToken cancellationToken = default);
        Task<MasterReference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MasterReference?> GetByCodeAsync(string category, string code, CancellationToken cancellationToken = default);
        Task<MasterReference> CreateAsync(MasterReference masterReference, Guid userId, CancellationToken cancellationToken = default);
        Task<MasterReference> UpdateAsync(MasterReference masterReference, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    }
}
