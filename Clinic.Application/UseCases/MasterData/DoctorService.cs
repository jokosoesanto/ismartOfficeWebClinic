using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.UseCases.MasterData
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorService(IDoctorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Doctor?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Doctor doctor)
        {
            await _repository.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            _repository.Update(doctor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, Guid deletedBy)
        {
            var doctor = await _repository.GetByIdAsync(id);
            if (doctor != null)
            {
                doctor.IsDeleted = true;
                doctor.DeletedAt = DateTime.UtcNow;
                doctor.DeletedBy = deletedBy;
                _repository.Update(doctor);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task CreateScheduleAsync(DoctorSchedule schedule)
        {
            await _repository.AddScheduleAsync(schedule);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateScheduleAsync(DoctorSchedule schedule)
        {
            _repository.UpdateSchedule(schedule);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteScheduleAsync(Guid scheduleId, Guid deletedBy)
        {
            var schedule = await _repository.GetScheduleByIdAsync(scheduleId);
            if (schedule != null)
            {
                schedule.IsDeleted = true;
                schedule.DeletedAt = DateTime.UtcNow;
                schedule.DeletedBy = deletedBy;
                _repository.UpdateSchedule(schedule);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}