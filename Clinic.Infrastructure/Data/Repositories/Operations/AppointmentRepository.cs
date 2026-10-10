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

        public async Task<Appointment?> GetByIdAsync(Guid id, bool includeDeleted = false)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.AppointmentType)
                .Include(a => a.Doctor)
                .Include(a => a.Location)
                .Include(a => a.Chair)
                .Include(a => a.Assistant)
                .Include(a => a.TreatmentItem)
                .Include(a => a.ChiefComplaints)
                .Include(a => a.VitalSign)
                .Include(a => a.ClinicalNote)
                .AsQueryable();

            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync(bool showCancelled = false)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.AppointmentType)
                .Include(a => a.Doctor)
                .Include(a => a.Location)
                .Include(a => a.Chair)
                .Include(a => a.Assistant)
                .Include(a => a.TreatmentItem)
                .Include(a => a.ChiefComplaints)
                .Include(a => a.VitalSign)
                .Include(a => a.ClinicalNote)
                .AsQueryable();

            if (showCancelled)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query.ToListAsync();
        }

        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
        }

        public void Update(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
        }

        public async Task<bool> HasOverlappingAppointmentAsync(Guid doctorId, DateTime date, TimeSpan startTime, TimeSpan? endTime, Guid? excludeAppointmentId = null)
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

            var safeEndTime = endTime ?? startTime.Add(TimeSpan.FromMinutes(1)); // Treat missing end time as minimal duration for overlap check
            return candidates.Any(a => a.StartTime < safeEndTime && (a.EndTime ?? a.StartTime.Add(TimeSpan.FromMinutes(1))) > startTime);
        }

        public async Task<bool> HasChairConflictAsync(Guid chairId, DateTime date, TimeSpan startTime, TimeSpan? endTime, Guid? excludeAppointmentId = null)
        {
            // Step 1: Filter by ChairId + Date in database.
            // Global query filter on Appointment already excludes IsDeleted records.
            var query = _context.Appointments
                .Include(a => a.AppointmentType)
                .Include(a => a.TreatmentItem)
                .Where(a => a.ChairId == chairId && a.Date == date);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            // Step 2: Materialize narrow candidate set, then evaluate
            // TimeSpan overlap in application memory (SQLite provider
            // cannot translate TimeSpan comparison operators).
            var candidates = await query
                .Select(a => new { 
                    a.StartTime, 
                    TypeCode = a.AppointmentType != null ? a.AppointmentType.Code : "",
                    TypeName = a.AppointmentType != null ? a.AppointmentType.Name : "",
                    TypeDuration = a.AppointmentType != null ? a.AppointmentType.DurationInMinutes : 0,
                    TreatmentDuration = a.TreatmentItem != null ? a.TreatmentItem.DurationInMinutes : 0
                })
                .ToListAsync();

            var safeEndTime = endTime ?? startTime;
            if (safeEndTime <= startTime) 
            {
                safeEndTime = startTime.Add(TimeSpan.FromMinutes(1));
            }
            
            return candidates.Any(a => {
                bool isTreatment = a.TypeCode == "TREATMENT" || a.TypeName.ToUpper().Contains("TREATMENT");
                int duration = isTreatment ? a.TreatmentDuration : a.TypeDuration;
                if (duration <= 0) duration = 1; // safe fallback
                var existingEffectiveEndTime = a.StartTime.Add(TimeSpan.FromMinutes(duration));
                return a.StartTime < safeEndTime && existingEffectiveEndTime > startTime;
            });
        }

        public async Task<bool> HasPatientConflictAsync(Guid patientId, DateTime date, TimeSpan startTime, TimeSpan? endTime, Guid? excludeAppointmentId = null)
        {
            var query = _context.Appointments
                .Include(a => a.AppointmentType)
                .Include(a => a.TreatmentItem)
                .Where(a => a.PatientId == patientId && a.Date == date);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            var candidates = await query
                .Select(a => new { 
                    a.StartTime, 
                    TypeCode = a.AppointmentType != null ? a.AppointmentType.Code : "",
                    TypeName = a.AppointmentType != null ? a.AppointmentType.Name : "",
                    TypeDuration = a.AppointmentType != null ? a.AppointmentType.DurationInMinutes : 0,
                    TreatmentDuration = a.TreatmentItem != null ? a.TreatmentItem.DurationInMinutes : 0
                })
                .ToListAsync();

            var safeEndTime = endTime ?? startTime;
            if (safeEndTime <= startTime) 
            {
                safeEndTime = startTime.Add(TimeSpan.FromMinutes(1));
            }
            
            return candidates.Any(a => {
                bool isTreatment = a.TypeCode == "TREATMENT" || a.TypeName.ToUpper().Contains("TREATMENT");
                int duration = isTreatment ? a.TreatmentDuration : a.TypeDuration;
                if (duration <= 0) duration = 1; // safe fallback
                var existingEffectiveEndTime = a.StartTime.Add(TimeSpan.FromMinutes(duration));
                return a.StartTime < safeEndTime && existingEffectiveEndTime > startTime;
            });
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDatesAsync(Guid doctorId, IEnumerable<DateTime> dates)
        {
            if (dates == null || !dates.Any()) return new List<Appointment>();

            var requestedDates = dates.Select(d => d.Date).ToHashSet();
            var minDate = requestedDates.Min();
            var maxDate = requestedDates.Max();

            // Step 1: Database-side narrow candidate query using Range.
            // Avoids EF Core's SQLite json_each translation mismatch on Date fields.
            var candidates = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Chair)
                .Where(a => a.DoctorId == doctorId && a.Date >= minDate && a.Date < maxDate.AddDays(1))
                .ToListAsync();

            // Step 2: Application-memory precise filtering
            return candidates
                .Where(a => requestedDates.Contains(a.Date.Date))
                .OrderBy(a => a.Date)
                .ThenBy(a => a.StartTime)
                .ToList();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.ClinicalNote)
                .Include(a => a.Location)
                .Include(a => a.Chair)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return appointments
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.StartTime)
                .ToList();
        }

        public async Task<Clinic.Domain.Entities.MasterData.AppointmentType?> GetAppointmentTypeByIdAsync(Guid id)
        {
            return await _context.AppointmentTypes.FindAsync(id);
        }
    }
}
