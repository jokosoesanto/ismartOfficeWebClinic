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
    }
}
