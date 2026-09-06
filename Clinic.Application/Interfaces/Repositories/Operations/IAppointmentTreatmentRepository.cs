using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Repositories.Operations
{
    public interface IAppointmentTreatmentRepository
    {
        Task<AppointmentTreatment?> GetByIdAsync(Guid id);
        Task<IEnumerable<AppointmentTreatment>> GetByAppointmentIdAsync(Guid appointmentId);
        Task<IEnumerable<AppointmentTreatment>> GetByAppointmentIdsAsync(IEnumerable<Guid> appointmentIds);
        Task AddAsync(AppointmentTreatment treatment);
    }
}
