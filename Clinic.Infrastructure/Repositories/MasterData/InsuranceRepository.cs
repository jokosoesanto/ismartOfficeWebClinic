using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories.MasterData
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly AppDbContext _context;

        public InsuranceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Insurance?> GetByIdAsync(Guid id)
        {
            return await _context.Insurances
                .Include(i => i.Group)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Insurance>> GetAllAsync()
        {
            return await _context.Insurances
                .Include(i => i.Group)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var query = _context.Insurances.Where(i => i.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(i => i.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task AddAsync(Insurance insurance)
        {
            await _context.Insurances.AddAsync(insurance);
        }

        public void Update(Insurance insurance)
        {
            _context.Insurances.Update(insurance);
        }

        public void Delete(Insurance insurance)
        {
            _context.Insurances.Remove(insurance);
        }
    }
}
