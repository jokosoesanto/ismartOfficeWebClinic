using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.ViewComponents.Prototypes
{
    public class PrototypeDashboardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
