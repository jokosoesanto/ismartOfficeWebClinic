using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IPatientCreditRepository
    {
        Task<PatientCredit> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<PatientCredit>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<PatientCredit> AddAsync(PatientCredit entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(PatientCredit entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(PatientCredit entity, CancellationToken cancellationToken = default);
    }
}
