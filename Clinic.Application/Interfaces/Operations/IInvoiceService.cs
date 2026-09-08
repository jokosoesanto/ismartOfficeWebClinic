using System;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IInvoiceService
    {
        Task<System.Collections.Generic.IEnumerable<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken = default);
        Task<Invoice> GenerateInvoiceAsync(Guid appointmentId, Guid userId, CancellationToken cancellationToken = default);
        Task<Invoice?> GetInvoiceByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    }
}
