using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Operations;
using Clinic.Application.Interfaces.Repositories.Operations;
using Clinic.Domain.Entities.Operations;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;

namespace Clinic.Application.UseCases.Operations
{
    public class PostTreatmentInstructionService : IPostTreatmentInstructionService
    {
        private readonly IPostTreatmentInstructionRepository _instructionRepository;
        private readonly IAppointmentTreatmentRepository _treatmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostTreatmentInstructionService(
            IPostTreatmentInstructionRepository instructionRepository,
            IAppointmentTreatmentRepository treatmentRepository,
            IUnitOfWork unitOfWork)
        {
            _instructionRepository = instructionRepository;
            _treatmentRepository = treatmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PostTreatmentInstruction>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _instructionRepository.GetByAppointmentIdAsync(appointmentId);
        }

        public async Task<PostTreatmentInstruction> AddInstructionAsync(Guid appointmentId, Guid patientId, Guid userId, string instructionText)
        {
            if (string.IsNullOrWhiteSpace(instructionText))
            {
                throw new ArgumentException("Instruction text cannot be empty.");
            }

            // Rule A — Valid clinical state: Must have at least 1 Executed treatment
            var treatments = await _treatmentRepository.GetByAppointmentIdAsync(appointmentId);
            bool hasExecutedTreatment = treatments.Any(t => t.Status == TreatmentStatus.Executed);

            if (!hasExecutedTreatment)
            {
                throw new InvalidOperationException("Post-treatment instructions can only be recorded for encounters with at least one executed treatment.");
            }

            var instruction = new PostTreatmentInstruction
            {
                AppointmentId = appointmentId,
                PatientId = patientId,
                InstructionText = instructionText,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _instructionRepository.AddAsync(instruction);
            await _unitOfWork.SaveChangesAsync();

            return instruction;
        }
    }
}
