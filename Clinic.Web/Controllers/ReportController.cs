using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Clinic.Web.Controllers
{
    [Authorize]
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
