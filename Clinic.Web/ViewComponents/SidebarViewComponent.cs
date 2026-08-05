using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.Interfaces.Navigation;
using System;
using System.Security.Claims;

namespace Clinic.Web.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly INavigationService _navigationService;

        public SidebarViewComponent(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = HttpContext.User.IsInRole("Administrator");
            
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                var menu = await _navigationService.GetAuthorizedMenuAsync(userId, isAdmin);
                return View(menu);
            }
            
            return View(new System.Collections.Generic.List<Clinic.Application.Navigation.NavigationItem>());
        }
    }
}
