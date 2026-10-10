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
        
        // Allocation
        Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalAllocatedByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalAllocatedByInvoiceLineIdAsync(Guid invoiceLineId, CancellationToken cancellationToken = default);
        Task AddAllocationAsync(PaymentAllocation allocation, CancellationToken cancellationToken = default);
        Task AddRefundAsync(PaymentRefund refund, CancellationToken cancellationToken = default);
    }
}
