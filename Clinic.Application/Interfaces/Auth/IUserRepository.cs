using System;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task UpdateAsync(User user);
        Task AddAsync(User user);
        Task<System.Collections.Generic.IEnumerable<User>> GetAllAsync();
        Task<System.Collections.Generic.IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId);
        Task<string?> GetPermissionVersionAsync(Guid userId);
        Task UpdateAllPermissionVersionsAsync(string newVersion);
    }
}
