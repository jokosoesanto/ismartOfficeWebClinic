using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(Guid id);
        Task CreateAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(Guid id, Guid deletedBy);
        
        Task CreateScheduleAsync(DoctorSchedule schedule);
        Task UpdateScheduleAsync(DoctorSchedule schedule);
        Task DeleteScheduleAsync(Guid scheduleId, Guid deletedBy);
    }
}