using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IPostTreatmentInstructionService
    {
        Task<IEnumerable<PostTreatmentInstruction>> GetByAppointmentIdAsync(Guid appointmentId);
        Task<PostTreatmentInstruction> AddInstructionAsync(Guid appointmentId, Guid patientId, Guid userId, string instructionText);
    }
}
