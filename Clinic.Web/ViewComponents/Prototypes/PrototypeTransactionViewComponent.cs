using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.ViewComponents.Prototypes
{
    public class PrototypeTransactionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
