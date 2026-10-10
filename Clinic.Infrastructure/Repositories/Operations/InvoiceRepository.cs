using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Domain.Entities.Operations;
using Clinic.Application.Interfaces.Operations;

namespace Clinic.Infrastructure.Repositories.Operations
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly Data.AppDbContext _context;

        public InvoiceRepository(Data.AppDbContext context)
        {
            _context = context;
        }

        public async Task<System.Collections.Generic.IEnumerable<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Patient)
                .Include(i => i.Payments)
                .ThenInclude(p => p.Refunds)
                .Include(i => i.Adjustments)
                .Where(i => !i.IsDeleted)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Invoice?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceLines)
                .ThenInclude(il => il.AppointmentTreatment)
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(i => i.AppointmentId == appointmentId && !i.IsDeleted, cancellationToken);
        }

        public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceLines)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
        {
            await _context.Invoices.AddAsync(invoice, cancellationToken);
        }

        public void Update(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
        }

        public async Task<InvoiceLine?> GetInvoiceLineByIdAsync(Guid invoiceLineId, CancellationToken cancellationToken = default)
        {
            return await _context.InvoiceLines
                .FirstOrDefaultAsync(il => il.Id == invoiceLineId && !il.IsDeleted, cancellationToken);
        }

        public async Task AddAdjustmentAsync(InvoiceAdjustment adjustment, CancellationToken cancellationToken = default)
        {
            await _context.InvoiceAdjustments.AddAsync(adjustment, cancellationToken);
        }

        public async Task<System.Collections.Generic.IEnumerable<InvoiceAdjustment>> GetAdjustmentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            return await _context.InvoiceAdjustments
                .Where(a => a.InvoiceId == invoiceId && !a.IsDeleted)
                .OrderByDescending(a => a.AdjustmentDate)
                .ToListAsync(cancellationToken);
        }
    }
}
