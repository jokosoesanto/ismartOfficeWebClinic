using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.Operations;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories.Operations
{
    public class PatientCreditRepository : IPatientCreditRepository
    {
        private readonly AppDbContext _context;

        public PatientCreditRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PatientCredit> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.PatientCredits
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<PatientCredit>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await _context.PatientCredits
                .Where(c => c.PatientId == patientId && !c.IsDeleted)
                .OrderByDescending(c => c.CreditDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<PatientCredit> AddAsync(PatientCredit entity, CancellationToken cancellationToken = default)
        {
            await _context.PatientCredits.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task UpdateAsync(PatientCredit entity, CancellationToken cancellationToken = default)
        {
            _context.PatientCredits.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(PatientCredit entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            _context.PatientCredits.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
