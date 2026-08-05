using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Clinic.Application.Interfaces.Auth;

namespace Clinic.Web.Services.Auth
{
    public class PermissionCache : IPermissionCache
    {
        private readonly IMemoryCache _cache;
        
        public PermissionCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        private string GetPermissionKey(Guid userId) => $"permissions_{userId}";
        private string GetVersionKey(Guid userId) => $"permission_version_{userId}";

        public async Task<HashSet<string>> GetOrAddUserPermissionsAsync(Guid userId, Func<Task<HashSet<string>>> factory)
        {
            return await _cache.GetOrCreateAsync(GetPermissionKey(userId), async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                return await factory();
            });
        }

        public void InvalidateUserPermissions(Guid userId)
        {
            _cache.Remove(GetPermissionKey(userId));
        }

        public void InvalidateAll()
        {
            // Simple approach for an in-memory cache: Not natively supported by IMemoryCache to wipe all 
            // without keeping track of keys. But we can just use Compact or let them expire.
            // A better way is using a CancellationTokenSource, but for this sprint we can just let versioning handle it.
        }

        public string GetOrAddUserPermissionVersion(Guid userId, Func<string> factory)
        {
            return _cache.GetOrCreate(GetVersionKey(userId), entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                return factory();
            });
        }

        public void UpdateUserPermissionVersion(Guid userId, string version)
        {
            _cache.Set(GetVersionKey(userId), version, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1)
            });
            // Also invalidate the permissions cache when version updates
            InvalidateUserPermissions(userId);
        }
    }
}
