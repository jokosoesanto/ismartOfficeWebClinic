using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Repositories.Operations;
using Clinic.Domain.Entities.Operations;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories.Operations
{
    public class PostTreatmentInstructionRepository : IPostTreatmentInstructionRepository
    {
        private readonly AppDbContext _context;

        public PostTreatmentInstructionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostTreatmentInstruction>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.PostTreatmentInstructions
                .AsNoTracking()
                .Include(i => i.Patient)
                .Where(i => i.AppointmentId == appointmentId && !i.IsDeleted)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<PostTreatmentInstruction> AddAsync(PostTreatmentInstruction instruction)
        {
            await _context.PostTreatmentInstructions.AddAsync(instruction);
            // Must not call SaveChangesAsync here, let the Service handle the UnitOfWork
            return instruction;
        }
    }
}
