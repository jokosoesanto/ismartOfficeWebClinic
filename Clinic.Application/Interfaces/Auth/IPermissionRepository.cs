using System;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IPermissionRepository
    {
        Task<System.Collections.Generic.IEnumerable<Permission>> GetAllAsync();
        Task<Permission?> GetByIdAsync(Guid id);
        Task UpdateAsync(Permission permission);
        Task AddAsync(Permission permission);
    }
}
