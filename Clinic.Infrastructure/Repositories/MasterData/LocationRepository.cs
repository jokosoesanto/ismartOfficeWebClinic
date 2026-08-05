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
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;

        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Location?> GetByIdAsync(Guid id)
        {
            return await _context.Locations
                .Include(l => l.Chairs.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
        }

        public async Task<Location?> GetByCodeAsync(string code)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.ClinicCode == code && !l.IsDeleted);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations
                .Include(l => l.Chairs.Where(c => !c.IsDeleted))
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.ClinicName)
                .ToListAsync();
        }

        public async Task AddAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
        }

        public Task UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            return Task.CompletedTask;
        }

        public async Task<bool> HasChairsAsync(Guid id)
        {
            return await _context.Chairs.AnyAsync(c => c.LocationId == id && !c.IsDeleted);
        }
    }
}
