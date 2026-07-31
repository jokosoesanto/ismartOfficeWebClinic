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
            return View(breadcrumbs);
        }
    }
}
