using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.ViewComponents.Prototypes
{
    public class PrototypeAdministrationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
