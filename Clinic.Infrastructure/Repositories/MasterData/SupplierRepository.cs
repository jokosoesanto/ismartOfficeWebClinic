using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.MasterData
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync(bool includeDeleted = false)
        {
            if (includeDeleted)
            {
                return await _context.Suppliers.IgnoreQueryFilters().ToListAsync();
            }
            return await _context.Suppliers.ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(Guid id)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Supplier?> GetByCodeAsync(string code)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(x => EF.Functions.Like(x.Code, code));
        }

        public async Task<Supplier> AddAsync(Supplier entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.Suppliers.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Supplier entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Suppliers.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
