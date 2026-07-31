using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    [Route("[controller]")]
    public class ReportController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var metadata = new UIMetadata
            {
                Title = "Reports",
                ModuleName = "Report",
                Mode = RenderingMode.Template
            };
            return View("Templates/ReportViewer", metadata);
        }
    }
}
