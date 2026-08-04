using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.Auth;
using Clinic.Domain.Entities.Auth;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.Auth
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<System.Collections.Generic.IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task AddAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
        }

        public async Task<Permission?> GetByIdAsync(Guid id)
        {
            return await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task UpdateAsync(Permission permission)
        {
            _context.Permissions.Update(permission);
            return Task.CompletedTask;
        }
    }
}
