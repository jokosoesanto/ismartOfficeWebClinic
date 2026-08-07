using System.Threading;
using System.Threading.Tasks;
using Clinic.Domain.Entities.System;

namespace Clinic.Application.Interfaces.Configuration
{
    public interface INumberSequenceRepository
    {
        Task<System.Collections.Generic.IEnumerable<NumberSequence>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<NumberSequence?> GetByCodeAsync(string sequenceCode, CancellationToken cancellationToken = default);
        Task UpdateAsync(NumberSequence sequence, CancellationToken cancellationToken = default);
        void DetachAll();
    }
}
