using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.UseCases.Operations
{
    public class PatientCreditService : IPatientCreditService
    {
        private readonly IPatientCreditRepository _repository;

        public PatientCreditService(IPatientCreditRepository repository)
        {
            _repository = repository;
        }

        public async Task<PatientCredit> CreateCreditAsync(Guid patientId, decimal amount, string? reason, Guid userId, CancellationToken cancellationToken = default)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Prepayment amount must be greater than zero.", nameof(amount));
            }

            var credit = new PatientCredit
            {
                PatientId = patientId,
                Amount = amount,
                Reason = reason,
                CreditDate = DateTime.UtcNow,
                CreatedBy = userId
            };

            return await _repository.AddAsync(credit, cancellationToken);
        }

        public async Task<IEnumerable<PatientCredit>> GetCreditsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await _repository.GetByPatientIdAsync(patientId, cancellationToken);
        }

        public async Task<decimal> GetAvailableCreditAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var credits = await _repository.GetByPatientIdAsync(patientId, cancellationToken);
            // Since Credit Allocation to Invoices is not yet implemented (B5 MVP scope),
            // the available credit is just the sum of all active credits.
            return credits.Sum(c => c.Amount);
        }

        public async Task<PatientCredit> GetCreditByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.GetByIdAsync(id, cancellationToken);
        }

        public async Task DeleteCreditAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var credit = await _repository.GetByIdAsync(id, cancellationToken);
            if (credit == null)
            {
                throw new InvalidOperationException("Credit not found.");
            }

            // In MVP, we can delete the credit directly since we don't have usage/allocation links yet.
            credit.DeletedBy = userId;
            await _repository.DeleteAsync(credit, cancellationToken);
        }
    }
}
