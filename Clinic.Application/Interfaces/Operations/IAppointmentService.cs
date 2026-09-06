using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAsync(AppointmentDto dto, Guid userId);
        Task<IEnumerable<AppointmentDto>> GetAllAsync(bool showCancelled = false);
        Task<AppointmentDto?> GetByIdAsync(Guid id, bool includeDeleted = false);
        Task<AppointmentDto> UpdateAsync(AppointmentDto dto, Guid userId);
        Task DeleteAsync(Guid id, Guid deletedBy);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorAndDatesAsync(Guid doctorId, IEnumerable<DateTime> dates);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientIdAsync(Guid patientId);
        Task<IEnumerable<Guid>> GetEligibleDoctorIdsForReassignmentAsync(Guid appointmentId);

        Task CheckInAsync(Guid id, Guid userId);
        Task CompleteVisitAsync(Guid id, Guid userId);

        Task AddChiefComplaintAsync(AppointmentChiefComplaintDto dto, Guid userId);
        Task RemoveChiefComplaintAsync(Guid appointmentId, Guid complaintId, Guid userId);

        Task<AppointmentVitalSignDto> SaveVitalSignAsync(AppointmentVitalSignDto dto, Guid userId);
        Task<AppointmentClinicalNoteDto> SaveClinicalNoteAsync(AppointmentClinicalNoteDto dto, Guid userId);
    }
}
