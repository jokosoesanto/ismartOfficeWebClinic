using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.MasterData
{
    public class DiagnosisMasterRepository : IDiagnosisMasterRepository
    {
        private readonly AppDbContext _context;

        public DiagnosisMasterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiagnosisMaster>> GetAllAsync()
        {
            return await _context.DiagnosisMasters
                .AsNoTracking()
                .OrderBy(d => d.DiagnosisCode)
                .ToListAsync();
        }

        public async Task<DiagnosisMaster?> GetByIdAsync(Guid id)
        {
            return await _context.DiagnosisMasters.FindAsync(id);
        }

        public async Task<DiagnosisMaster?> GetByCodeAsync(string code)
        {
            return await _context.DiagnosisMasters
                .FirstOrDefaultAsync(d => d.DiagnosisCode == code);
        }

        public async Task<DiagnosisMaster> AddAsync(DiagnosisMaster diagnosisMaster)
        {
            _context.DiagnosisMasters.Add(diagnosisMaster);
            await _context.SaveChangesAsync();
            return diagnosisMaster;
        }

        public async Task UpdateAsync(DiagnosisMaster diagnosisMaster)
        {
            _context.DiagnosisMasters.Update(diagnosisMaster);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, Guid? deletedBy = null)
        {
            var diagnosisMaster = await _context.DiagnosisMasters.FindAsync(id);
            if (diagnosisMaster != null)
            {
                diagnosisMaster.IsDeleted = true;
                diagnosisMaster.DeletedAt = DateTime.UtcNow;
                diagnosisMaster.DeletedBy = deletedBy;
                _context.DiagnosisMasters.Update(diagnosisMaster);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.DiagnosisMasters.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.DiagnosisMasters.AnyAsync(e => e.DiagnosisCode == code && e.Id != excludeId.Value);
            }
            return await _context.DiagnosisMasters.AnyAsync(e => e.DiagnosisCode == code);
        }
    }
}
