using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.Operations;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories.Operations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Refunds)
                .Where(p => p.InvoiceId == invoiceId && !p.IsDeleted)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalPaidByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId && !p.IsDeleted)
                .SumAsync(p => p.Amount, cancellationToken);
        }

        public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            await _context.Payments.AddAsync(payment, cancellationToken);
        }

        public async Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Refunds)
                .FirstOrDefaultAsync(p => p.Id == paymentId && !p.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentAllocations
                .Include(a => a.InvoiceLine)
                .Where(a => a.PaymentId == paymentId && !a.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalAllocatedByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentAllocations
                .Where(a => a.PaymentId == paymentId && !a.IsDeleted)
                .SumAsync(a => a.AllocatedAmount, cancellationToken);
        }

        public async Task<decimal> GetTotalAllocatedByInvoiceLineIdAsync(Guid invoiceLineId, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentAllocations
                .Where(a => a.InvoiceLineId == invoiceLineId && !a.IsDeleted)
                .SumAsync(a => a.AllocatedAmount, cancellationToken);
        }

        public async Task AddAllocationAsync(PaymentAllocation allocation, CancellationToken cancellationToken = default)
        {
            await _context.PaymentAllocations.AddAsync(allocation, cancellationToken);
        }

        public async Task AddRefundAsync(PaymentRefund refund, CancellationToken cancellationToken = default)
        {
            await _context.PaymentRefunds.AddAsync(refund, cancellationToken);
        }
    }
}
