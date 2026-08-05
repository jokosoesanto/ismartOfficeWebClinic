using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Auth;

namespace Clinic.Application.UseCases.Auth
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionCache _permissionCache;
        private readonly IUserRepository _userRepository;

        public PermissionService(IPermissionCache permissionCache, IUserRepository userRepository)
        {
            _permissionCache = permissionCache;
            _userRepository = userRepository;
        }

        public async Task<HashSet<string>> GetEffectivePermissionsAsync(Guid userId)
        {
            var currentVersion = await GetUserPermissionVersionAsync(userId);
            var cachedVersion = _permissionCache.GetOrAddUserPermissionVersion(userId, () => currentVersion ?? string.Empty);
            
            if (currentVersion != null && cachedVersion != currentVersion)
            {
                _permissionCache.UpdateUserPermissionVersion(userId, currentVersion);
                // The cache will automatically invalidate the permissions when version is updated
            }

            return await _permissionCache.GetOrAddUserPermissionsAsync(userId, async () =>
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return new HashSet<string>();
                }

                // Proper filtering logic: Active Roles and Active Permissions
                var permissions = user.UserRoles
                    .Where(ur => ur.Role.IsActive)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Where(rp => rp.Permission.IsActive && !string.IsNullOrEmpty(rp.Permission.Code))
                    .Select(rp => rp.Permission.Code)
                    .Distinct()
                    .ToHashSet();
                
                Console.WriteLine($"FORENSIC_DB: Loaded user {userId} from DB.");
                Console.WriteLine($"FORENSIC_DB: User.IsActive = {user.IsActive}");
                Console.WriteLine($"FORENSIC_DB: UserRoles Count = {user.UserRoles.Count}");
                foreach(var ur in user.UserRoles)
                {
                    Console.WriteLine($"FORENSIC_DB:   Role {ur.Role.Name} (IsActive: {ur.Role.IsActive}), RolePermissions Count: {ur.Role.RolePermissions.Count}");
                    foreach(var rp in ur.Role.RolePermissions)
                    {
                        Console.WriteLine($"FORENSIC_DB:     Permission Code={rp.Permission.Code}, Name={rp.Permission.Name}, IsActive={rp.Permission.IsActive}");
                    }
                }
                
                return permissions;
            });
        }

        public async Task<string?> GetUserPermissionVersionAsync(Guid userId)
        {
            return await _userRepository.GetPermissionVersionAsync(userId);
        }
    }
}
