using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.Auth;
using Clinic.Domain.Entities.Auth;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.Auth
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted && r.IsActive);
        }

        public async Task<System.Collections.Generic.IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Where(r => !r.IsDeleted && r.IsActive)
                .Include(r => r.UserRoles)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .ToListAsync();
        }

        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted && r.IsActive);
        }

        public Task UpdateAsync(Role role)
        {
            if (string.Equals(role.Name, "Administrator", StringComparison.OrdinalIgnoreCase) && !role.IsActive)
            {
                throw new InvalidOperationException("Administrator role cannot be deactivated.");
            }
            _context.Roles.Update(role);
            return Task.CompletedTask;
        }
    }
}
