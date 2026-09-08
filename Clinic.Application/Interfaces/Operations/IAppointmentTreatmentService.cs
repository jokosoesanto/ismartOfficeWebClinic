using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IAppointmentTreatmentService
    {
        Task<AppointmentTreatmentDto> CreateTreatmentAsync(AppointmentTreatmentDto dto, Guid userId);
        Task<IEnumerable<AppointmentTreatmentDto>> GetTreatmentsByAppointmentIdAsync(Guid appointmentId);
        Task<IEnumerable<AppointmentTreatmentDto>> GetTreatmentsByAppointmentIdsAsync(IEnumerable<Guid> appointmentIds);
        Task<(bool Success, string Message)> DeleteTreatmentAsync(Guid id);
    }
}
