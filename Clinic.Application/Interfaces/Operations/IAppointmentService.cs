using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAsync(AppointmentDto dto, Guid userId);
        Task<IEnumerable<AppointmentDto>> GetAllAsync();
        Task<AppointmentDto?> GetByIdAsync(Guid id);
        Task<AppointmentDto> UpdateAsync(AppointmentDto dto, Guid userId);
        Task DeleteAsync(Guid id, Guid deletedBy);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorAndDatesAsync(Guid doctorId, IEnumerable<DateTime> dates);
        Task<IEnumerable<Guid>> GetEligibleDoctorIdsForReassignmentAsync(Guid appointmentId);
    }
}
