using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IPermissionService
    {
        Task<HashSet<string>> GetEffectivePermissionsAsync(Guid userId);
        Task<string?> GetUserPermissionVersionAsync(Guid userId);
    }
}
