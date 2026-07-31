using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    [Route("[controller]")]
    public class MedicalRecordController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var metadata = new UIMetadata
            {
                Title = "Medical Records",
                ModuleName = "MedicalRecord",
                Mode = RenderingMode.Template
            };
            return View("Templates/MR_Dashboard", metadata);
        }

        [HttpGet("Chart/{id}")]
        public IActionResult Chart(string id)
        {
            var metadata = new UIMetadata
            {
                Title = "Patient Chart",
                ModuleName = "MedicalRecord",
                Mode = RenderingMode.Template
            };
            return View("Templates/MR_Chart", metadata);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var metadata = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
            return View("Templates/MR_TreatmentForm", metadata);
        }

        [HttpGet("History/{id}")]
        public IActionResult History(string id)
        {
            var metadata = new UIMetadata { Title = "Treatment History", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
            return View("Templates/MR_History", metadata);
        }

        [HttpGet("{id}")]
        public IActionResult Details(string id)
        {
            var metadata = new UIMetadata { Title = "Treatment Detail", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
            return View("Templates/MR_Detail", metadata);
        }
    }
}
