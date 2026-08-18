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
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IChairRepository _chairRepository;
        private readonly IDoctorLeaveRequestRepository _doctorLeaveRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            ILocationRepository locationRepository,
            IChairRepository chairRepository,
            IDoctorLeaveRequestRepository doctorLeaveRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _locationRepository = locationRepository;
            _chairRepository = chairRepository;
            _doctorLeaveRequestRepository = doctorLeaveRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentDto> CreateAsync(AppointmentDto dto, Guid userId)
        {
            // Validate references
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null) throw new InvalidOperationException("Invalid Patient");

            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
            if (doctor == null) throw new InvalidOperationException("Invalid Doctor");
            if (!doctor.IsActive) throw new InvalidOperationException("Selected Doctor is inactive and cannot be assigned new appointments.");

            var location = await _locationRepository.GetByIdAsync(dto.LocationId);
            if (location == null) throw new InvalidOperationException("Invalid Location");

            // Validates chair exists AND belongs to the specified Location
            var chair = await _chairRepository.GetByIdAsync(dto.ChairId);
            if (chair == null) throw new InvalidOperationException("Invalid Chair");
            if (chair.LocationId != dto.LocationId) throw new InvalidOperationException("Selected Chair does not belong to the selected Location");

            bool hasOverlap = await _appointmentRepository.HasOverlappingAppointmentAsync(dto.DoctorId, dto.Date, dto.StartTime, dto.EndTime);
            if (hasOverlap)
            {
                throw new InvalidOperationException("Doctor is already booked for the selected time. Please choose another time.");
            }

            var conflictingLeaves = await _doctorLeaveRequestRepository.GetDuplicateDatesAsync(dto.DoctorId, new[] { dto.Date });
            if (conflictingLeaves.Any())
            {
                throw new InvalidOperationException("Doctor is on leave on the selected date. Please choose another date or doctor.");
            }

            bool hasChairConflict = await _appointmentRepository.HasChairConflictAsync(dto.ChairId, dto.Date, dto.StartTime, dto.EndTime);
            if (hasChairConflict)
            {
                throw new InvalidOperationException("Chair is already booked for the selected time. Please choose another chair or time.");
            }

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                LocationId = dto.LocationId,
                ChairId = dto.ChairId,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = Clinic.Domain.Enums.AppointmentStatus.Schedule,
                Notes = dto.Notes,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _appointmentRepository.Add(appointment);
            await _unitOfWork.SaveChangesAsync();

            dto.Id = appointment.Id;
            return dto;
        }

        public async Task<AppointmentDto> UpdateAsync(AppointmentDto dto, Guid userId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(dto.Id);
            if (appointment == null) throw new InvalidOperationException("Appointment not found");

            // Validate references
            // Patient validation is intentionally omitted here to enforce Patient Immutability.
            // An existing appointment belongs to its original patient; changes are not permitted.


            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
            if (doctor == null) throw new InvalidOperationException("Invalid Doctor");
            if (!doctor.IsActive) throw new InvalidOperationException("Selected Doctor is inactive and cannot be assigned appointments.");

            var location = await _locationRepository.GetByIdAsync(dto.LocationId);
            if (location == null) throw new InvalidOperationException("Invalid Location");

            // Validates chair exists AND belongs to the specified Location
            var chair = await _chairRepository.GetByIdAsync(dto.ChairId);
            if (chair == null) throw new InvalidOperationException("Invalid Chair");
            if (chair.LocationId != dto.LocationId) throw new InvalidOperationException("Selected Chair does not belong to the selected Location");

            bool hasOverlap = await _appointmentRepository.HasOverlappingAppointmentAsync(dto.DoctorId, dto.Date, dto.StartTime, dto.EndTime, dto.Id);
            if (hasOverlap)
            {
                throw new InvalidOperationException("Doctor is already booked for the selected time. Please choose another time.");
            }

            var conflictingLeaves = await _doctorLeaveRequestRepository.GetDuplicateDatesAsync(dto.DoctorId, new[] { dto.Date });
            if (conflictingLeaves.Any())
            {
                throw new InvalidOperationException("Doctor is on leave on the selected date. Please choose another date or doctor.");
            }

            bool hasChairConflict = await _appointmentRepository.HasChairConflictAsync(dto.ChairId, dto.Date, dto.StartTime, dto.EndTime, dto.Id);
            if (hasChairConflict)
            {
                throw new InvalidOperationException("Chair is already booked for the selected time. Please choose another chair or time.");
            }

            // appointment.PatientId is intentionally NOT updated to enforce immutability.
            appointment.DoctorId = dto.DoctorId;
            appointment.LocationId = dto.LocationId;
            appointment.ChairId = dto.ChairId;
            appointment.Date = dto.Date;
            appointment.StartTime = dto.StartTime;
            appointment.EndTime = dto.EndTime;
            appointment.Notes = dto.Notes;
            appointment.UpdatedBy = userId;
            appointment.UpdatedAt = DateTime.UtcNow;

            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return dto;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAsync(bool showCancelled = false)
        {
            var appointments = await _appointmentRepository.GetAllAsync(showCancelled);
            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient!.FullName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor!.FullName,
                LocationId = a.LocationId,
                LocationName = a.Location!.ClinicName,
                ChairId = a.ChairId,
                ChairName = a.Chair!.Name,
                Date = a.Date,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Notes = a.Notes,
                IsDeleted = a.IsDeleted
            });
        }

        public async Task<AppointmentDto?> GetByIdAsync(Guid id, bool includeDeleted = false)
        {
            var a = await _appointmentRepository.GetByIdAsync(id, includeDeleted);
            if (a == null) return null;

            return new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient!.FullName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor!.FullName,
                LocationId = a.LocationId,
                LocationName = a.Location!.ClinicName,
                ChairId = a.ChairId,
                ChairName = a.Chair!.Name,
                Date = a.Date,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Notes = a.Notes,
                IsDeleted = a.IsDeleted
            };
        }

        public async Task DeleteAsync(Guid id, Guid deletedBy)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment != null)
            {
                appointment.IsDeleted = true;
                appointment.DeletedAt = DateTime.UtcNow;
                appointment.DeletedBy = deletedBy;
                
                _appointmentRepository.Update(appointment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorAndDatesAsync(Guid doctorId, IEnumerable<DateTime> dates)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorAndDatesAsync(doctorId, dates);
            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient?.FullName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor?.FullName,
                LocationId = a.LocationId,
                LocationName = a.Location?.ClinicName,
                ChairId = a.ChairId,
                ChairName = a.Chair?.Name,
                Date = a.Date,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Notes = a.Notes
            });
        }

        public async Task<IEnumerable<Guid>> GetEligibleDoctorIdsForReassignmentAsync(Guid appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null) throw new InvalidOperationException("Appointment not found");

            var allDoctors = await _doctorRepository.GetAllAsync();
            var activeDoctorIds = allDoctors
                .Where(d => !d.IsDeleted && d.IsActive && d.Id != appointment.DoctorId)
                .Select(d => d.Id)
                .ToList();

            var eligibleIds = new List<Guid>();

            foreach (var docId in activeDoctorIds)
            {
                var leaves = await _doctorLeaveRequestRepository.GetDuplicateDatesAsync(docId, new[] { appointment.Date });
                if (leaves.Any()) continue;

                bool hasOverlap = await _appointmentRepository.HasOverlappingAppointmentAsync(docId, appointment.Date, appointment.StartTime, appointment.EndTime, appointmentId);
                if (hasOverlap) continue;

                eligibleIds.Add(docId);
            }

            return eligibleIds;
        }
    }
}
