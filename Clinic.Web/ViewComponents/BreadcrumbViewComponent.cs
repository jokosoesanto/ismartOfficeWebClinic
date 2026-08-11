using Microsoft.AspNetCore.Mvc;
using Clinic.Application.Navigation;
using Microsoft.AspNetCore.Http.Extensions;
using System;

namespace Clinic.Web.ViewComponents
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        private readonly INavigationProvider _navigationProvider;

        public BreadcrumbViewComponent(INavigationProvider navigationProvider)
        {
            _navigationProvider = navigationProvider;
        }

        public IViewComponentResult Invoke()
        {
            var path = Request.Path.Value;
            if (string.IsNullOrEmpty(path)) path = "/";
            
            var breadcrumbs = _navigationProvider.GetBreadcrumbs(path);
            
            // Enterprise Breadcrumb Standard: Dashboard must be Level 1
            if (breadcrumbs.Count > 0 && breadcrumbs[0].Id != "dashboard")
            {
                breadcrumbs.Insert(0, new NavigationItem 
                { 
                    Id = "dashboard", 
                    Title = "Dashboard", 
                    Route = "/", 
                    BreadcrumbTitle = "Dashboard" 
                });
            }

            if (ViewContext?.ViewData["DynamicBreadcrumbs"] is System.Collections.Generic.List<NavigationItem> dynamicBreadcrumbs)
            {
                breadcrumbs.AddRange(dynamicBreadcrumbs);
            }
            
            // Append current page context if it's a child route not explicitly in the menu
            var last = breadcrumbs.LastOrDefault();
            if (last != null && !last.Route.Equals(path, StringComparison.OrdinalIgnoreCase) && path != "/")
            {
                var pageTitle = ViewContext?.ViewData["Title"]?.ToString();
                if (!string.IsNullOrEmpty(pageTitle) && !pageTitle.Equals(last.BreadcrumbTitle, StringComparison.OrdinalIgnoreCase))
                {
                    breadcrumbs.Add(new NavigationItem 
                    { 
                        Title = pageTitle, 
                        BreadcrumbTitle = pageTitle, 
                        Route = path 
                    });
                }
            }

            return View(breadcrumbs);
        }
    }
}
