using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Domain.Entities.System;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IMasterReferenceRepository
    {
        Task<IEnumerable<MasterReference>> GetByCategoryAsync(string category, bool activeOnly = true, CancellationToken cancellationToken = default);
        Task<MasterReference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MasterReference?> GetByCodeAsync(string category, string code, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(string category, string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
        Task AddAsync(MasterReference masterReference, CancellationToken cancellationToken = default);
        Task UpdateAsync(MasterReference masterReference, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    }
}
