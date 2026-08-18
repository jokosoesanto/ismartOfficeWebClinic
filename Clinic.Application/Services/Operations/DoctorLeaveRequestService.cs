using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Services.Operations
{
    public class DoctorLeaveRequestService : IDoctorLeaveRequestService
    {
        private readonly IDoctorLeaveRequestRepository _repository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorLeaveRequestService(
            IDoctorLeaveRequestRepository repository,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorLeaveRequestDto> CreateAsync(DoctorLeaveRequestDto dto, Guid userId)
        {
            // Validate doctor
            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
            if (doctor == null) throw new InvalidOperationException("Invalid Doctor");

            if (dto.LeaveDates == null || !dto.LeaveDates.Any())
                throw new InvalidOperationException("At least one leave date is required.");

            // Normalize dates to date-only
            var normalizedDates = dto.LeaveDates.Select(d => d.Date).Distinct().ToList();

            // Check duplicate dates for this doctor across all active leave requests
            var duplicates = await _repository.GetDuplicateDatesAsync(dto.DoctorId, normalizedDates);
            if (duplicates.Any())
            {
                var dateList = string.Join(", ", duplicates.Select(d => d.ToString("dd MMM yyyy")));
                throw new InvalidOperationException($"Doctor already has approved leave on: {dateList}. Please remove duplicate dates.");
            }

            // Validate no past dates
            var today = DateTime.UtcNow.Date;
            var pastDates = normalizedDates.Where(d => d < today).ToList();
            if (pastDates.Any())
            {
                var dateList = string.Join(", ", pastDates.Select(d => d.ToString("dd MMM yyyy")));
                throw new InvalidOperationException($"Cannot create leave for past dates: {dateList}. Leave dates must be today or in the future.");
            }

            var request = new DoctorLeaveRequest
            {
                Id = Guid.NewGuid(),
                DoctorId = dto.DoctorId,
                Reason = dto.Reason,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var date in normalizedDates)
            {
                request.LeaveDates.Add(new DoctorLeaveDate
                {
                    Id = Guid.NewGuid(),
                    DoctorLeaveRequestId = request.Id,
                    Date = date
                });
            }

            _repository.Add(request);
            await _unitOfWork.SaveChangesAsync();

            dto.Id = request.Id;
            return dto;
        }

        public async Task<DoctorLeaveRequestDto> UpdateAsync(DoctorLeaveRequestDto dto, Guid userId)
        {
            var request = await _repository.GetByIdAsync(dto.Id);
            if (request == null) throw new InvalidOperationException("Leave request not found.");

            if (request.LeaveDates.Any() && request.LeaveDates.Min(d => d.Date.Date) <= DateTime.UtcNow.Date)
                throw new InvalidOperationException("Doctor Leave Request cannot be edited because the leave has already started or contains a past leave date.");

            if (dto.LeaveDates == null || !dto.LeaveDates.Any())
                throw new InvalidOperationException("At least one leave date is required.");

            // Normalize dates to date-only
            var normalizedDates = dto.LeaveDates.Select(d => d.Date).Distinct().ToList();

            // Check duplicate dates for this doctor, excluding the current request
            var duplicates = await _repository.GetDuplicateDatesAsync(request.DoctorId, normalizedDates, request.Id);
            if (duplicates.Any())
            {
                var dateList = string.Join(", ", duplicates.Select(d => d.ToString("dd MMM yyyy")));
                throw new InvalidOperationException($"Doctor already has approved leave on: {dateList}. Please remove duplicate dates.");
            }

            // Validate no past dates
            var today = DateTime.UtcNow.Date;
            var pastDates = normalizedDates.Where(d => d < today).ToList();
            if (pastDates.Any())
            {
                var dateList = string.Join(", ", pastDates.Select(d => d.ToString("dd MMM yyyy")));
                throw new InvalidOperationException($"Cannot create leave for past dates: {dateList}. Leave dates must be today or in the future.");
            }

            // Update reason
            request.Reason = dto.Reason;
            request.UpdatedBy = userId;
            request.UpdatedAt = DateTime.UtcNow;

            // Reconcile child dates: produce exactly the submitted set
            var existingDates = request.LeaveDates.ToList();
            var existingDateValues = existingDates.Select(d => d.Date.Date).ToHashSet();
            var requestedDateValues = normalizedDates.ToHashSet();

            // Remove dates no longer in the request (preserve cancelled dates)
            var toRemove = existingDates.Where(d => !d.IsCancelled && !requestedDateValues.Contains(d.Date.Date)).ToList();
            foreach (var item in toRemove)
            {
                request.LeaveDates.Remove(item);
            }

            // Add new dates not already present
            var toAdd = requestedDateValues.Except(existingDateValues).ToList();
            foreach (var date in toAdd)
            {
                request.LeaveDates.Add(new DoctorLeaveDate
                {
                    Id = Guid.Empty, // Explicitly set to empty to ensure EF Core tracks it as EntityState.Added
                    DoctorLeaveRequestId = request.Id,
                    Date = date
                });
            }
            await _unitOfWork.SaveChangesAsync();

            return dto;
        }

        public async Task<IEnumerable<DoctorLeaveRequestDto>> GetAllAsync()
        {
            var requests = await _repository.GetAllAsync();
            return requests.Select(r => new DoctorLeaveRequestDto
            {
                Id = r.Id,
                DoctorId = r.DoctorId,
                DoctorName = r.Doctor?.FullName,
                Reason = r.Reason,
                LeaveDates = r.LeaveDates.Where(d => !d.IsCancelled).OrderBy(d => d.Date).Select(d => d.Date).ToList(),
                LeaveDateDetails = r.LeaveDates.Select(d => new DoctorLeaveDateDto
                {
                    Id = d.Id,
                    Date = d.Date,
                    IsCancelled = d.IsCancelled,
                    CancelledAt = d.CancelledAt,
                    CancelledBy = d.CancelledBy,
                    CancellationReason = d.CancellationReason
                }).OrderBy(d => d.Date).ToList(),
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<DoctorLeaveRequestDto?> GetByIdAsync(Guid id)
        {
            var r = await _repository.GetByIdAsync(id);
            if (r == null) return null;

            return new DoctorLeaveRequestDto
            {
                Id = r.Id,
                DoctorId = r.DoctorId,
                DoctorName = r.Doctor?.FullName,
                Reason = r.Reason,
                LeaveDates = r.LeaveDates.Where(d => !d.IsCancelled).OrderBy(d => d.Date).Select(d => d.Date).ToList(),
                LeaveDateDetails = r.LeaveDates.Select(d => new DoctorLeaveDateDto
                {
                    Id = d.Id,
                    Date = d.Date,
                    IsCancelled = d.IsCancelled,
                    CancelledAt = d.CancelledAt,
                    CancelledBy = d.CancelledBy,
                    CancellationReason = d.CancellationReason
                }).OrderBy(d => d.Date).ToList(),
                CreatedAt = r.CreatedAt
            };
        }

        public async Task DeleteAsync(Guid id, Guid deletedBy)
        {
            var request = await _repository.GetByIdAsync(id);
            if (request != null)
            {
                if (request.LeaveDates.Any(d => !d.IsCancelled && d.Date.Date <= DateTime.UtcNow.Date))
                {
                    throw new InvalidOperationException("Cannot delete a leave request that has already started or contains historical dates.");
                }

                request.IsDeleted = true;
                request.DeletedAt = DateTime.UtcNow;
                request.DeletedBy = deletedBy;

                _repository.Update(request);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task CancelLeaveDateAsync(Guid leaveDateId, string? reason, Guid cancelledBy)
        {
            var request = await _repository.GetByLeaveDateIdAsync(leaveDateId);
            if (request == null) throw new InvalidOperationException("Leave date not found.");

            if (request.IsDeleted) throw new InvalidOperationException("Cannot cancel a date on a deleted leave request.");

            var leaveDate = request.LeaveDates.FirstOrDefault(d => d.Id == leaveDateId);
            if (leaveDate == null) throw new InvalidOperationException("Leave date not found in request.");

            if (leaveDate.IsCancelled) throw new InvalidOperationException("Leave date is already cancelled.");

            if (leaveDate.Date.Date <= DateTime.UtcNow.Date)
                throw new InvalidOperationException("Cannot cancel a historical or current leave date. Only future dates can be cancelled.");

            leaveDate.IsCancelled = true;
            leaveDate.CancelledAt = DateTime.UtcNow;
            leaveDate.CancelledBy = cancelledBy;
            leaveDate.CancellationReason = reason;

            // Notice we DO NOT use _repository.Update(request) as it would re-update the aggregate
            // and we had a previous concurrency fix. We just let EF track the changes on leaveDate 
            // since GetByLeaveDateIdAsync includes LeaveDates and it's tracked by DbContext.
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
