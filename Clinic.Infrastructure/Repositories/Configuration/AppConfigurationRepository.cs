using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Domain.Entities.Configuration;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.Configuration
{
    public class AppConfigurationRepository : IAppConfigurationRepository
    {
        private readonly AppDbContext _context;

        public AppConfigurationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppConfiguration>> GetAllAsync()
        {
            return await _context.Set<AppConfiguration>().ToListAsync();
        }

        public async Task<IEnumerable<AppConfiguration>> GetByCategoryAsync(string category)
        {
            return await _context.Set<AppConfiguration>()
                .Where(c => c.Category == category)
                .ToListAsync();
        }

        public async Task<AppConfiguration?> GetByKeyAsync(string key)
        {
            return await _context.Set<AppConfiguration>()
                .FirstOrDefaultAsync(c => c.Key == key);
        }

        public async Task AddAsync(AppConfiguration config)
        {
            await _context.Set<AppConfiguration>().AddAsync(config);
        }

        public Task UpdateAsync(AppConfiguration config)
        {
            _context.Set<AppConfiguration>().Update(config);
            return Task.CompletedTask;
        }
    }
}

