using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Repositories.Operations
{
    public interface IAppointmentDiagnosisRepository
    {
        Task<AppointmentDiagnosis?> GetByIdAsync(Guid id);
        Task<IEnumerable<AppointmentDiagnosis>> GetByAppointmentIdAsync(Guid appointmentId);
        Task AddAsync(AppointmentDiagnosis entity);
        Task UpdateAsync(AppointmentDiagnosis entity);
    }
}
