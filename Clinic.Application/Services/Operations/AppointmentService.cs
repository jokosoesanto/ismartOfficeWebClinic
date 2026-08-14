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
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            ILocationRepository locationRepository,
            IChairRepository chairRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _locationRepository = locationRepository;
            _chairRepository = chairRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentDto> CreateAsync(AppointmentDto dto, Guid userId)
        {
            // Validate references
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null) throw new InvalidOperationException("Invalid Patient");

            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
            if (doctor == null) throw new InvalidOperationException("Invalid Doctor");

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

        public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
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
                Notes = a.Notes
            });
        }
    }
}
