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

            return View(breadcrumbs);
        }
    }
}
