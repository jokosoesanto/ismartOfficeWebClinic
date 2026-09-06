using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IAppointmentDiagnosisService
    {
        Task<AppointmentDiagnosisDto> CreateDiagnosisAsync(AppointmentDiagnosisDto dto, Guid userId);
        Task<IEnumerable<AppointmentDiagnosisDto>> GetDiagnosesByAppointmentIdAsync(Guid appointmentId);
        Task RemoveDiagnosisAsync(Guid appointmentId, Guid diagnosisId, Guid userId);
    }
}
