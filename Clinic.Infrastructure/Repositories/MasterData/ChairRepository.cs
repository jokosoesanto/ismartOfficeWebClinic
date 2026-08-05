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
    public class ChairRepository : IChairRepository
    {
        private readonly AppDbContext _context;

        public ChairRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Chair?> GetByIdAsync(Guid id)
        {
            return await _context.Chairs
                .Include(c => c.Location)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Chair?> GetByCodeAsync(string code)
        {
            return await _context.Chairs
                .FirstOrDefaultAsync(c => c.Code == code && !c.IsDeleted);
        }

        public async Task<IEnumerable<Chair>> GetAllAsync()
        {
            return await _context.Chairs
                .Include(c => c.Location)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Chair chair)
        {
            await _context.Chairs.AddAsync(chair);
        }

        public Task UpdateAsync(Chair chair)
        {
            _context.Chairs.Update(chair);
            return Task.CompletedTask;
        }

        public Task<bool> HasAppointmentsAsync(Guid id)
        {
            // Placeholder for now, assume false until Appointment module is created
            return Task.FromResult(false);
        }
    }
}
