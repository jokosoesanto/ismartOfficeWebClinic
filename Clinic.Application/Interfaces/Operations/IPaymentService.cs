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
        Task<PaymentAllocation> AllocatePaymentAsync(Guid paymentId, Guid invoiceLineId, decimal amount, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<PaymentRefund> CreateRefundAsync(Guid paymentId, decimal amount, string? reason, Guid userId, CancellationToken cancellationToken = default);
    }
}
