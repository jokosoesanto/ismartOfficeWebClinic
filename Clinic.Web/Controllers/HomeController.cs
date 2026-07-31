using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var metadata = new UIMetadata
            {
                Title = "Dashboard",
                ModuleName = "Dashboard",
                Mode = RenderingMode.Template
            };
            return View("Templates/Dashboard", metadata);
        }
    }
}
