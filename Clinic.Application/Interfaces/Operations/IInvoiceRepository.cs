using System;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IInvoiceRepository
    {
        Task<System.Collections.Generic.IEnumerable<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken = default);
        Task<Invoice?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
        Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
        void Update(Invoice invoice);
    }
}
