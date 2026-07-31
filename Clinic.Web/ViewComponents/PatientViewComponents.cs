using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.ViewComponents
{
    public class PatientSummaryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string patientId)
        {
            ViewData["PatientId"] = patientId;
            return View();
        }
    }

    public class PatientTabsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }

    public class MedicalAlertViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
