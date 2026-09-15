using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Repositories.Operations
{
    public interface IPostTreatmentInstructionRepository
    {
        Task<IEnumerable<PostTreatmentInstruction>> GetByAppointmentIdAsync(Guid appointmentId);
        Task<PostTreatmentInstruction> AddAsync(PostTreatmentInstruction instruction);
    }
}
