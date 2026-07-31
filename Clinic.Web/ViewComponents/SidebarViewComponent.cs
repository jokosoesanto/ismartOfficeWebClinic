using Microsoft.AspNetCore.Mvc;
using Clinic.Application.Navigation;

namespace Clinic.Web.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly INavigationProvider _navigationProvider;

        public SidebarViewComponent(INavigationProvider navigationProvider)
        {
            _navigationProvider = navigationProvider;
        }

        public IViewComponentResult Invoke()
        {
            var menu = _navigationProvider.GetNavigationMenu();
            return View(menu);
        }
    }
}
