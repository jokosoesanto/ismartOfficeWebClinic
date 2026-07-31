using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.ViewComponents
{
    public class PatientFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
