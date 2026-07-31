using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.ViewComponents
{
    public class DataGridViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(object modelData, string title, string icon)
        {
            ViewData["Title"] = title;
            ViewData["Icon"] = icon;
            return View(modelData);
        }
    }
}
