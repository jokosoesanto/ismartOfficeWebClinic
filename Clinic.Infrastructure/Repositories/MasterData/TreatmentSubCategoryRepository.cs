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
    public class TreatmentSubCategoryRepository : ITreatmentSubCategoryRepository
    {
        private readonly AppDbContext _context;

        public TreatmentSubCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TreatmentSubCategory?> GetByIdAsync(Guid id)
        {
            return await _context.TreatmentSubCategories
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<TreatmentSubCategory>> GetAllAsync()
        {
            return await _context.TreatmentSubCategories
                .Include(x => x.Category)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<TreatmentSubCategory>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _context.TreatmentSubCategories
                .Include(x => x.Category)
                .Where(x => x.CategoryId == categoryId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public async Task AddAsync(TreatmentSubCategory subCategory)
        {
            await _context.TreatmentSubCategories.AddAsync(subCategory);
        }

        public async Task UpdateAsync(TreatmentSubCategory subCategory)
        {
            _context.TreatmentSubCategories.Update(subCategory);
            await Task.CompletedTask;
        }

        public async Task<bool> IsNameUniqueAsync(Guid categoryId, string name, Guid? excludeId = null)
        {
            var query = _context.TreatmentSubCategories
                .Where(c => !c.IsDeleted && c.CategoryId == categoryId && c.SubCategoryName == name);
                
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}
