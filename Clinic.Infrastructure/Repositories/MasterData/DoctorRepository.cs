using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.MasterData
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.Specialty)
                .Include(d => d.PrimaryLocation)
                .Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAllActiveAsync()
        {
            return await _context.Doctors
                .Include(d => d.Specialty)
                .Include(d => d.PrimaryLocation)
                .Where(x => !x.IsDeleted && x.IsActive).ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(Guid id)
        {
            return await _context.Doctors
                .Include(d => d.Specialty)
                .Include(d => d.PrimaryLocation)
                .Include(d => d.Schedules.Where(s => !s.IsDeleted)).ThenInclude(s => s.Location)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
        }

        public void Update(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
        }

        public async Task<DoctorSchedule?> GetScheduleByIdAsync(Guid scheduleId)
        {
            return await _context.DoctorSchedules.FindAsync(scheduleId);
        }

        public async Task AddScheduleAsync(DoctorSchedule schedule)
        {
            await _context.DoctorSchedules.AddAsync(schedule);
        }

        public void UpdateSchedule(DoctorSchedule schedule)
        {
            _context.DoctorSchedules.Update(schedule);
        }

        public void DeleteSchedule(DoctorSchedule schedule)
        {
            _context.DoctorSchedules.Remove(schedule);
        }
    }
}