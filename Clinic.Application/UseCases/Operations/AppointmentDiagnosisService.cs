using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Operations;
using Clinic.Application.Interfaces.Repositories.Operations;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.Operations;
using Clinic.Domain.Enums;

namespace Clinic.Application.UseCases.Operations
{
    public class AppointmentDiagnosisService : IAppointmentDiagnosisService
    {
        private readonly IAppointmentDiagnosisRepository _diagnosisRepository;
        private readonly IAppointmentService _appointmentService;
        private readonly IDiagnosisMasterService _diagnosisMasterService;
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentDiagnosisService(
            IAppointmentDiagnosisRepository diagnosisRepository,
            IAppointmentService appointmentService,
            IDiagnosisMasterService diagnosisMasterService,
            IUnitOfWork unitOfWork)
        {
            _diagnosisRepository = diagnosisRepository;
            _appointmentService = appointmentService;
            _diagnosisMasterService = diagnosisMasterService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentDiagnosisDto> CreateDiagnosisAsync(AppointmentDiagnosisDto dto, Guid userId)
        {
            var appointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }
            
            if (appointment.Status != AppointmentStatus.OnTime && appointment.Status != AppointmentStatus.Completed)
            {
                throw new InvalidOperationException("Diagnoses can only be modified when the appointment is checked-in or completed.");
            }

            var diagnosisMaster = await _diagnosisMasterService.GetByIdAsync(dto.DiagnosisMasterId);
            if (diagnosisMaster == null)
            {
                throw new KeyNotFoundException("Diagnosis Master not found.");
            }
            
            if (!diagnosisMaster.IsActive)
            {
                throw new InvalidOperationException("Cannot use an inactive diagnosis.");
            }
            
            var existingDiagnoses = await _diagnosisRepository.GetByAppointmentIdAsync(dto.AppointmentId);
            if (existingDiagnoses.Any(d => d.DiagnosisMasterId == dto.DiagnosisMasterId))
            {
                throw new InvalidOperationException("This diagnosis has already been added to the appointment.");
            }

            var entity = new AppointmentDiagnosis
            {
                Id = Guid.NewGuid(),
                AppointmentId = dto.AppointmentId,
                DiagnosisMasterId = dto.DiagnosisMasterId,
                Remark = dto.Remark,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            await _diagnosisRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.DiagnosisCode = diagnosisMaster.DiagnosisCode;
            dto.DiagnosisName = diagnosisMaster.DiagnosisName;

            return dto;
        }

        public async Task<IEnumerable<AppointmentDiagnosisDto>> GetDiagnosesByAppointmentIdAsync(Guid appointmentId)
        {
            var diagnoses = await _diagnosisRepository.GetByAppointmentIdAsync(appointmentId);
            return diagnoses.Select(d => new AppointmentDiagnosisDto
            {
                Id = d.Id,
                AppointmentId = d.AppointmentId,
                DiagnosisMasterId = d.DiagnosisMasterId,
                DiagnosisCode = d.DiagnosisMaster?.DiagnosisCode,
                DiagnosisName = d.DiagnosisMaster?.DiagnosisName,
                Remark = d.Remark
            }).ToList();
        }

        public async Task RemoveDiagnosisAsync(Guid appointmentId, Guid diagnosisId, Guid userId)
        {
            var appointment = await _appointmentService.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }
            
            if (appointment.Status != AppointmentStatus.OnTime && appointment.Status != AppointmentStatus.Completed)
            {
                throw new InvalidOperationException("Diagnoses can only be modified when the appointment is checked-in or completed.");
            }
            
            var diagnosis = await _diagnosisRepository.GetByIdAsync(diagnosisId);
            if (diagnosis == null || diagnosis.AppointmentId != appointmentId)
            {
                throw new KeyNotFoundException("Diagnosis not found in this appointment.");
            }
            
            diagnosis.IsDeleted = true;
            diagnosis.DeletedAt = DateTime.UtcNow;
            diagnosis.DeletedBy = userId;
            
            await _diagnosisRepository.UpdateAsync(diagnosis);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
