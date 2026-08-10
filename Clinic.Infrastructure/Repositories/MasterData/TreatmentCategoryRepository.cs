using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Infrastructure.Repositories.MasterData
{
    public class TreatmentCategoryRepository : ITreatmentCategoryRepository
    {
        private readonly AppDbContext _context;

        public TreatmentCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TreatmentCategory?> GetByIdAsync(Guid id)
        {
            return await _context.TreatmentCategories
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<IEnumerable<TreatmentCategory>> GetAllAsync()
        {
            return await _context.TreatmentCategories
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.CategoryName)
                .ToListAsync();
        }

        public async Task<IEnumerable<TreatmentCategory>> GetActiveCategoriesAsync()
        {
            return await _context.TreatmentCategories
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.CategoryName)
                .ToListAsync();
        }

        public async Task AddAsync(TreatmentCategory category)
        {
            await _context.TreatmentCategories.AddAsync(category);
        }

        public Task UpdateAsync(TreatmentCategory category)
        {
            _context.TreatmentCategories.Update(category);
            return Task.CompletedTask;
        }

        public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null)
        {
            return !await _context.TreatmentCategories
                .AnyAsync(x => x.CategoryName == name && (!excludeId.HasValue || x.Id != excludeId.Value) && !x.IsDeleted);
        }
    }
}
