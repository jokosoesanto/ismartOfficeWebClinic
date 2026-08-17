using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Appointment>> GetAllAsync();
        void Add(Appointment appointment);
        void Update(Appointment appointment);
        Task<bool> HasOverlappingAppointmentAsync(Guid doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null);
        Task<bool> HasChairConflictAsync(Guid chairId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDatesAsync(Guid doctorId, IEnumerable<DateTime> dates);
    }
}
