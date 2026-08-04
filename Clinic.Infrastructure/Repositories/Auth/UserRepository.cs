using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.Auth;
using Clinic.Domain.Entities.Auth;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.Auth
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var normalized = username.ToUpperInvariant();
            return await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.NormalizedUsername == normalized);
        }

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
    }
}
