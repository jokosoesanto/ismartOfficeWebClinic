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
    public class TreatmentCatalogRepository : ITreatmentCatalogRepository
    {
        private readonly AppDbContext _context;

        public TreatmentCatalogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TreatmentCatalog>> GetAllAsync()
        {
            return await _context.TreatmentCatalogs
                .Include(t => t.Category)
                .Include(t => t.SubCategory)
                .Include(t => t.ServiceType)
                .OrderBy(t => t.TreatmentName)
                .ToListAsync();
        }

        public async Task<IEnumerable<TreatmentCatalog>> GetBySubCategoryIdAsync(Guid subCategoryId)
        {
            return await _context.TreatmentCatalogs
                .Include(t => t.Category)
                .Include(t => t.SubCategory)
                .Include(t => t.ServiceType)
                .Where(t => t.SubCategoryId == subCategoryId)
                .OrderBy(t => t.TreatmentName)
                .ToListAsync();
        }

        public async Task<TreatmentCatalog?> GetByIdAsync(Guid id)
        {
            return await _context.TreatmentCatalogs
                .Include(t => t.Category)
                .Include(t => t.SubCategory)
                .Include(t => t.ServiceType)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(TreatmentCatalog entity)
        {
            await _context.TreatmentCatalogs.AddAsync(entity);
        }

        public Task UpdateAsync(TreatmentCatalog entity)
        {
            _context.TreatmentCatalogs.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> IsNameUniqueAsync(Guid subCategoryId, string name, Guid? excludeId = null)
        {
            var query = _context.TreatmentCatalogs.Where(t => t.SubCategoryId == subCategoryId && t.TreatmentName == name);
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }
    }
}
