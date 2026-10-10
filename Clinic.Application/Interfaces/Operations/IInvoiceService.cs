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
        Task<InvoiceAdjustment> CreateAdjustmentAsync(Guid invoiceId, decimal amount, string adjustmentType, string? reason, Guid userId, CancellationToken cancellationToken = default);
        Task<System.Collections.Generic.IEnumerable<InvoiceAdjustment>> GetAdjustmentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    }
}
