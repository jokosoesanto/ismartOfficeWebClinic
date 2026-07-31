using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    [Route("[controller]")]
    public class AppointmentController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var metadata = new UIMetadata
            {
                Title = "Appointments",
                ModuleName = "Appointment",
                Mode = RenderingMode.Template
            };
            return View("Templates/Scheduler", metadata);
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            var metadata = new UIMetadata { Title = "Add Appointment", ModuleName = "Appointment", Mode = RenderingMode.Template };
            return View("Templates/Scheduler_Form", metadata);
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(string id)
        {
            var metadata = new UIMetadata { Title = "Edit Appointment", ModuleName = "Appointment", Mode = RenderingMode.Template };
            return View("Templates/Scheduler_Form", metadata);
        }

        [HttpGet("{id}")]
        public IActionResult Details(string id)
        {
            var metadata = new UIMetadata { Title = "Appointment Detail", ModuleName = "Appointment", Mode = RenderingMode.Template };
            return View("Templates/Scheduler_Detail", metadata);
        }
    }
}
