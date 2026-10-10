using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IPatientCreditService
    {
        Task<PatientCredit> CreateCreditAsync(Guid patientId, decimal amount, string? reason, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PatientCredit>> GetCreditsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<decimal> GetAvailableCreditAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<PatientCredit> GetCreditByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteCreditAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    }
}
