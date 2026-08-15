using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.DTOs.Operations;
using Clinic.Application.Interfaces.Operations;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.Operations
{
    public class ScheduleBoardRepository : IScheduleBoardRepository
    {
        private readonly AppDbContext _context;

        public ScheduleBoardRepository(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<ScheduleBoardDto> BuildBaseQuery()
        {
            return _context.DoctorSchedules
                .Where(s => !s.IsDeleted && !s.Doctor.IsDeleted)
                .Select(s => new ScheduleBoardDto
                {
                    ScheduleId = s.Id,
                    DoctorId = s.DoctorId,
                    DoctorName = s.Doctor.Title != null ? (s.Doctor.Title + " " + s.Doctor.FullName) : s.Doctor.FullName,
                    Specialty = s.Doctor.Specialty.Name,
                    DoctorColor = s.Doctor.Color,
                    LocationId = s.LocationId,
                    LocationName = s.Location.ClinicName,
                    Chair = "-", // Chair is currently not mapped on Schedule entity
                    DayOfWeek = s.DayOfWeek,
                    DayName = s.DayOfWeek == 0 ? "Sunday" :
                              s.DayOfWeek == 1 ? "Monday" :
                              s.DayOfWeek == 2 ? "Tuesday" :
                              s.DayOfWeek == 3 ? "Wednesday" :
                              s.DayOfWeek == 4 ? "Thursday" :
                              s.DayOfWeek == 5 ? "Friday" : "Saturday",
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    BreakStart = s.BreakStart,
                    BreakEnd = s.BreakEnd,
                    IsActive = s.Doctor.IsActive,
                    IsAvailable = s.IsAvailable,
                    Status = s.Doctor.IsActive 
                        ? (s.IsAvailable ? "Available" : "Leave")
                        : "Inactive"
                });
        }

        public async Task<IEnumerable<ScheduleBoardDto>> GetAllSchedulesForDataTablesAsync()
        {
            // Provides all base data for DataTables client-side / server-side processing
            return await BuildBaseQuery().ToListAsync();
        }

        public async Task<IEnumerable<ScheduleBoardDto>> GetSchedulesAsync(
            Guid? locationId, 
            Guid? doctorId, 
            Guid? specialtyId, 
            int? dayOfWeek,
            DateTime? specificDate,
            string? searchKeyword)
        {
            var query = BuildBaseQuery();

            if (locationId.HasValue && locationId.Value != Guid.Empty)
                query = query.Where(x => x.LocationId == locationId.Value);

            if (doctorId.HasValue && doctorId.Value != Guid.Empty)
                query = query.Where(x => x.DoctorId == doctorId.Value);

            if (specialtyId.HasValue && specialtyId.Value != Guid.Empty)
                query = query.Where(x => _context.Doctors.Any(d => d.Id == x.DoctorId && d.SpecialtyId == specialtyId.Value));

            if (specificDate.HasValue)
            {
                var day = (int)specificDate.Value.DayOfWeek;
                query = query.Where(x => x.DayOfWeek == day);
            }
            else if (dayOfWeek.HasValue)
            {
                query = query.Where(x => x.DayOfWeek == dayOfWeek.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                var lowerSearch = searchKeyword.ToLower();
                query = query.Where(x => 
                    x.DoctorName.ToLower().Contains(lowerSearch) ||
                    (x.Specialty != null && x.Specialty.ToLower().Contains(lowerSearch)) ||
                    x.LocationName.ToLower().Contains(lowerSearch));
            }

            var results = await query.ToListAsync();

            // Populate specific date if requested
            if (specificDate.HasValue)
            {
                foreach (var item in results)
                {
                    item.SpecificDate = specificDate;
                }
            }

            // Also fetch Active Appointments to overlay on the Schedule Board
            if (specificDate.HasValue)
            {
                var apptQuery = _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                    .Include(a => a.Location)
                    .Include(a => a.Chair)
                    .Where(a => !a.IsDeleted && a.Date == specificDate.Value);

                if (locationId.HasValue && locationId.Value != Guid.Empty)
                    apptQuery = apptQuery.Where(a => a.LocationId == locationId.Value);

                if (doctorId.HasValue && doctorId.Value != Guid.Empty)
                    apptQuery = apptQuery.Where(a => a.DoctorId == doctorId.Value);

                if (specialtyId.HasValue && specialtyId.Value != Guid.Empty)
                    apptQuery = apptQuery.Where(a => a.Doctor.SpecialtyId == specialtyId.Value);

                var appointments = await apptQuery.ToListAsync();

                foreach (var a in appointments)
                {
                    results.Add(new ScheduleBoardDto
                    {
                        ScheduleId = a.Id, 
                        DoctorId = a.DoctorId,
                        DoctorName = a.Doctor.Title != null ? (a.Doctor.Title + " " + a.Doctor.FullName) : a.Doctor.FullName,
                        Specialty = a.Doctor.Specialty?.Name,
                        DoctorColor = a.Doctor.Color,
                        LocationId = a.LocationId,
                        LocationName = a.Location.ClinicName,
                        Chair = a.Chair?.Name ?? "-",
                        DayOfWeek = (int)a.Date.DayOfWeek,
                        DayName = a.Date.DayOfWeek.ToString(),
                        SpecificDate = a.Date,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        IsActive = true,
                        IsAvailable = false,
                        Status = "Appointment", 
                        IsAppointment = true,
                        PatientName = a.Patient!.FullName,
                        AppointmentStatus = a.Status.ToString()
                    });
                }
            }

            return results;
        }
    }
}
