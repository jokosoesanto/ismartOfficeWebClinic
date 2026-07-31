using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.ViewComponents.Prototypes
{
    public class PrototypeReportViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
