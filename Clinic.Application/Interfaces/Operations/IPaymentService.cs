using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(Guid invoiceId, decimal amount, string paymentMethod, string? referenceNumber, string? notes, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalPaidByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    }
}
