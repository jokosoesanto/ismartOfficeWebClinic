using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.MasterData
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly AppDbContext _context;

        public SpecialtyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Specialty>> GetAllAsync()
        {
            return await _context.Specialties.Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Specialty>> GetAllActiveAsync()
        {
            return await _context.Specialties.Where(x => !x.IsDeleted && x.IsActive).ToListAsync();
        }

        public async Task<Specialty?> GetByIdAsync(Guid id)
        {
            return await _context.Specialties.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<Specialty?> GetByCodeAsync(string code)
        {
            return await _context.Specialties.FirstOrDefaultAsync(x => x.Code == code && !x.IsDeleted);
        }

        public async Task AddAsync(Specialty specialty)
        {
            await _context.Specialties.AddAsync(specialty);
        }

        public void Update(Specialty specialty)
        {
            _context.Specialties.Update(specialty);
        }
    }
}