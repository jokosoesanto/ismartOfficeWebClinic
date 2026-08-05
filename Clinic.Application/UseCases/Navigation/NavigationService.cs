using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.Navigation;
using Clinic.Application.Interfaces.Navigation;
using Clinic.Application.Interfaces.Auth;

namespace Clinic.Application.UseCases.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly INavigationProvider _navigationProvider;
        private readonly IPermissionService _permissionService;

        public NavigationService(INavigationProvider navigationProvider, IPermissionService permissionService)
        {
            _navigationProvider = navigationProvider;
            _permissionService = permissionService;
        }

        public async Task<List<NavigationItem>> GetAuthorizedMenuAsync(Guid userId, bool isAdmin)
        {
            var menu = _navigationProvider.GetNavigationMenu();
            
            if (isAdmin)
            {
                return menu; // Admins see everything
            }

            var permissions = await _permissionService.GetEffectivePermissionsAsync(userId);
            
            Console.WriteLine($"FORENSIC_NAV: User {userId} (IsAdmin: {isAdmin})");
            Console.WriteLine($"FORENSIC_NAV: Permissions count: {permissions.Count}");
            Console.WriteLine($"FORENSIC_NAV: Permissions List: {string.Join(", ", permissions)}");

            var result = new List<NavigationItem>();

            foreach (var item in menu)
            {
                // First filter children
                var authorizedChildren = new List<NavigationItem>();
                if (item.Children != null && item.Children.Any())
                {
                    foreach (var child in item.Children)
                    {
                        if (string.IsNullOrEmpty(child.RequiredPermission) || permissions.Contains(child.RequiredPermission))
                        {
                            authorizedChildren.Add(child);
                        }
                    }
                }

                // Parent is shown if:
                // 1. It has no RequiredPermission AND has no children
                // 2. It has RequiredPermission AND user possesses it
                // 3. It has authorized children
                
                bool hasDirectAccess = string.IsNullOrEmpty(item.RequiredPermission) || permissions.Contains(item.RequiredPermission);
                bool hasChildAccess = authorizedChildren.Any();

                if (hasDirectAccess || hasChildAccess)
                {
                    // Clone item to avoid modifying singleton provider
                    var clone = new NavigationItem
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Icon = item.Icon,
                        Route = item.Route,
                        BreadcrumbTitle = item.BreadcrumbTitle,
                        Description = item.Description,
                        RequiredPermission = item.RequiredPermission,
                        Children = authorizedChildren
                    };
                    result.Add(clone);
                }
            }

            return result;
        }
    }
}
