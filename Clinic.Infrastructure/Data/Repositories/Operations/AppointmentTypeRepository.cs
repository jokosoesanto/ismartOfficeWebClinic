using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Infrastructure.Data.Repositories.Operations
{
    public class AppointmentTypeRepository : IAppointmentTypeRepository
    {
        private readonly AppDbContext _context;

        public AppointmentTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppointmentType>> GetAllAsync()
        {
            return await _context.AppointmentTypes
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<AppointmentType?> GetByIdAsync(Guid id)
        {
            return await _context.AppointmentTypes
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<AppointmentType?> GetByCodeAsync(string code)
        {
            return await _context.AppointmentTypes
                .FirstOrDefaultAsync(a => a.Code == code && !a.IsDeleted);
        }

        public async Task AddAsync(AppointmentType entity)
        {
            await _context.AppointmentTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AppointmentType entity)
        {
            _context.AppointmentTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.AppointmentTypes
                .AnyAsync(a => a.Id == id && !a.IsDeleted);
        }
    }
}
