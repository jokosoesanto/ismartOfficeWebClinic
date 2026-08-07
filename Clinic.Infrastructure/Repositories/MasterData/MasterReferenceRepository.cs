using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.System;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories.MasterData
{
    public class MasterReferenceRepository : IMasterReferenceRepository
    {
        private readonly AppDbContext _context;

        public MasterReferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MasterReference>> GetByCategoryAsync(string category, bool activeOnly = true, CancellationToken cancellationToken = default)
        {
            var query = _context.MasterReferences.Where(x => x.Category == category);
            if (activeOnly)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<MasterReference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.MasterReferences.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<MasterReference?> GetByCodeAsync(string category, string code, CancellationToken cancellationToken = default)
        {
            return await _context.MasterReferences.FirstOrDefaultAsync(x => x.Category == category && x.Code == code, cancellationToken);
        }

        public async Task<bool> AnyAsync(string category, string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.MasterReferences.Where(x => x.Category == category && x.Code == code);
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            return await query.AnyAsync(cancellationToken);
        }

        public async Task AddAsync(MasterReference masterReference, CancellationToken cancellationToken = default)
        {
            await _context.MasterReferences.AddAsync(masterReference, cancellationToken);
        }

        public Task UpdateAsync(MasterReference masterReference, CancellationToken cancellationToken = default)
        {
            _context.MasterReferences.Update(masterReference);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.MasterReferences
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(cancellationToken);
        }
    }
}
