using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Operations;
using Clinic.Application.Interfaces.Repositories.Operations;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.UseCases.Operations
{
    public class AppointmentTreatmentService : IAppointmentTreatmentService
    {
        private readonly IAppointmentTreatmentRepository _treatmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentTreatmentService(
            IAppointmentTreatmentRepository treatmentRepository,
            IUnitOfWork unitOfWork)
        {
            _treatmentRepository = treatmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentTreatmentDto> CreateTreatmentAsync(AppointmentTreatmentDto dto, Guid userId)
        {
            var treatment = new AppointmentTreatment
            {
                AppointmentId = dto.AppointmentId,
                TreatmentItemId = dto.TreatmentItemId,
                SiteNumber = dto.SiteNumber,
                SiteDetail = dto.SiteDetail,
                ActualPrice = dto.ActualPrice,
                Remark = dto.Remark,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _treatmentRepository.AddAsync(treatment);
            await _unitOfWork.SaveChangesAsync();

            var savedTreatment = await _treatmentRepository.GetByIdAsync(treatment.Id);
            
            return MapToDto(savedTreatment!);
        }

        public async Task<IEnumerable<AppointmentTreatmentDto>> GetTreatmentsByAppointmentIdAsync(Guid appointmentId)
        {
            var treatments = await _treatmentRepository.GetByAppointmentIdAsync(appointmentId);
            return treatments.Select(MapToDto).ToList();
        }

        private AppointmentTreatmentDto MapToDto(AppointmentTreatment entity)
        {
            return new AppointmentTreatmentDto
            {
                Id = entity.Id,
                AppointmentId = entity.AppointmentId,
                TreatmentItemId = entity.TreatmentItemId,
                TreatmentItemName = entity.TreatmentItem?.TreatmentName,
                SiteNumber = entity.SiteNumber,
                SiteDetail = entity.SiteDetail,
                ActualPrice = entity.ActualPrice,
                Remark = entity.Remark,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
