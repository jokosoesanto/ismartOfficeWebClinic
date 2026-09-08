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
    }
}
