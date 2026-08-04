using System;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetByIdAsync(Guid id);
        Task<System.Collections.Generic.IEnumerable<Role>> GetAllAsync();
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
    }
}
