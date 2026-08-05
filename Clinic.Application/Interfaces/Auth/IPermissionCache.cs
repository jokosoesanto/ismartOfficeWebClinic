using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IPermissionCache
    {
        Task<HashSet<string>> GetOrAddUserPermissionsAsync(Guid userId, Func<Task<HashSet<string>>> factory);
        void InvalidateUserPermissions(Guid userId);
        void InvalidateAll();
        string GetOrAddUserPermissionVersion(Guid userId, Func<string> factory);
        void UpdateUserPermissionVersion(Guid userId, string version);
    }
}
