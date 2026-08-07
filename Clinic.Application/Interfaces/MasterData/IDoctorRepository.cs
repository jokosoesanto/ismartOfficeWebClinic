using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<IEnumerable<Doctor>> GetAllActiveAsync();
        Task<Doctor?> GetByIdAsync(Guid id);
        Task AddAsync(Doctor doctor);
        void Update(Doctor doctor);
        Task<DoctorSchedule?> GetScheduleByIdAsync(Guid scheduleId);
        Task AddScheduleAsync(DoctorSchedule schedule);
        void UpdateSchedule(DoctorSchedule schedule);
        void DeleteSchedule(DoctorSchedule schedule);
    }
}