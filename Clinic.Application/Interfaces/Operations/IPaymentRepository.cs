using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalPaidByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
        Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    }
}
