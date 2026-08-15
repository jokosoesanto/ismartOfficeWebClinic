using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Data.Repositories.Operations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Location)
                .Include(a => a.Chair)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Location)
                .Include(a => a.Chair)
                .ToListAsync();
        }

        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
        }

        public void Update(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
        }

        public async Task<bool> HasOverlappingAppointmentAsync(Guid doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null)
        {
            // Step 1: Filter by DoctorId + Date in database.
            // Global query filter on Appointment already excludes IsDeleted records.
            var query = _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Date == date);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            // Step 2: Materialize narrow candidate set, then evaluate
            // TimeSpan overlap in application memory (SQLite provider
            // cannot translate TimeSpan comparison operators).
            var candidates = await query
                .Select(a => new { a.StartTime, a.EndTime })
                .ToListAsync();

            return candidates.Any(a => a.StartTime < endTime && a.EndTime > startTime);
        }

        public async Task<bool> HasChairConflictAsync(Guid chairId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null)
        {
            // Step 1: Filter by ChairId + Date in database.
            // Global query filter on Appointment already excludes IsDeleted records.
            var query = _context.Appointments
                .Where(a => a.ChairId == chairId && a.Date == date);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            // Step 2: Materialize narrow candidate set, then evaluate
            // TimeSpan overlap in application memory (SQLite provider
            // cannot translate TimeSpan comparison operators).
            var candidates = await query
                .Select(a => new { a.StartTime, a.EndTime })
                .ToListAsync();

            return candidates.Any(a => a.StartTime < endTime && a.EndTime > startTime);
        }
    }
}
