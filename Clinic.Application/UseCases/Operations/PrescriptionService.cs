using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Operations;
using Clinic.Application.Interfaces.Repositories.Operations;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.UseCases.Operations
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork)
        {
            _prescriptionRepository = prescriptionRepository;
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string Message)> CreatePrescriptionAsync(Guid appointmentId, Guid patientId, Guid prescriberId, List<PrescriptionItemDto> items)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                return (false, "Appointment not found.");
            }

            if (appointment.PatientId != patientId)
            {
                return (false, "Patient and Appointment relationship is invalid.");
            }

            if (items == null || !items.Any())
            {
                return (false, "Prescription must contain at least one medication item.");
            }

            var prescription = new Prescription
            {
                AppointmentId = appointmentId,
                PatientId = patientId,
                PrescribedByUserId = prescriberId,
                PrescribedAt = DateTime.UtcNow,
                Status = "Active",
                CreatedBy = prescriberId
            };

            foreach (var itemDto in items)
            {
                if (string.IsNullOrWhiteSpace(itemDto.MedicationName) ||
                    string.IsNullOrWhiteSpace(itemDto.Dosage) ||
                    string.IsNullOrWhiteSpace(itemDto.Frequency) ||
                    string.IsNullOrWhiteSpace(itemDto.Instruction))
                {
                    return (false, "Medication name, dosage, frequency, and instruction are required for all items.");
                }

                prescription.Items.Add(new PrescriptionItem
                {
                    MedicationName = itemDto.MedicationName,
                    Dosage = itemDto.Dosage,
                    Frequency = itemDto.Frequency,
                    Instruction = itemDto.Instruction,
                    CreatedBy = prescriberId
                });
            }

            await _prescriptionRepository.AddAsync(prescription);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Prescription created successfully.");
        }
    }
}
