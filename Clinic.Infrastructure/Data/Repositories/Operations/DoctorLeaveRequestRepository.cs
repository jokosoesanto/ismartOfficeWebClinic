using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Data.Repositories.Operations
{
    public class DoctorLeaveRequestRepository : IDoctorLeaveRequestRepository
    {
        private readonly AppDbContext _context;

        public DoctorLeaveRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DoctorLeaveRequest?> GetByIdAsync(Guid id)
        {
            return await _context.DoctorLeaveRequests
                .Include(r => r.Doctor)
                .Include(r => r.LeaveDates)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<DoctorLeaveRequest?> GetByLeaveDateIdAsync(Guid leaveDateId)
        {
            return await _context.DoctorLeaveRequests
                .Include(r => r.Doctor)
                .Include(r => r.LeaveDates)
                .FirstOrDefaultAsync(r => r.LeaveDates.Any(d => d.Id == leaveDateId));
        }

        public async Task<IEnumerable<DoctorLeaveRequest>> GetAllAsync()
        {
            return await _context.DoctorLeaveRequests
                .Include(r => r.Doctor)
                .Include(r => r.LeaveDates)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public void Add(DoctorLeaveRequest request)
        {
            _context.DoctorLeaveRequests.Add(request);
        }

        public void Update(DoctorLeaveRequest request)
        {
            _context.DoctorLeaveRequests.Update(request);
        }

        public async Task<List<DateTime>> GetDuplicateDatesAsync(Guid doctorId, IEnumerable<DateTime> dates, Guid? excludeRequestId = null)
        {
            var dateList = dates.Select(d => d.Date).ToList();

            var query = _context.DoctorLeaveDates
                .Where(d => d.DoctorLeaveRequest != null
                    && d.DoctorLeaveRequest.DoctorId == doctorId
                    && !d.DoctorLeaveRequest.IsDeleted
                    && !d.IsCancelled);

            if (excludeRequestId.HasValue)
            {
                query = query.Where(d => d.DoctorLeaveRequestId != excludeRequestId.Value);
            }

            // Materialize the existing leave dates for this doctor, then compare in memory
            // (SQLite does not translate .Contains on DateTime well in all cases)
            var existingDates = await query.Select(d => d.Date).ToListAsync();
            var existingDateSet = existingDates.Select(d => d.Date).ToHashSet();

            return dateList.Where(d => existingDateSet.Contains(d)).ToList();
        }

        public async Task<List<DateTime>> GetDatesWithAppointmentsAsync(Guid doctorId, IEnumerable<DateTime> dates)
        {
            var dateList = dates.Select(d => d.Date).ToList();

            // Get all non-deleted appointment dates for this doctor
            var appointmentDates = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.Date)
                .ToListAsync();

            var appointmentDateSet = appointmentDates.Select(d => d.Date).ToHashSet();

            return dateList.Where(d => appointmentDateSet.Contains(d)).ToList();
        }
    }
}
