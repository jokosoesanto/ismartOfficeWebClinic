using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Clinic.Domain.Entities.System;

namespace Clinic.Application.Interfaces.Configuration
{
    public interface INumberSequenceService
    {
        Task<string> GenerateSequenceAsync(string sequenceCode, CancellationToken cancellationToken = default);
        Task<IEnumerable<NumberSequence>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<string> PreviewNextNumberAsync(string sequenceCode, CancellationToken cancellationToken = default);
    }
}
