using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.ViewComponents.Prototypes
{
    public class PrototypeMasterDetailViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
