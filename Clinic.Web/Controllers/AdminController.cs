using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    [Route("[controller]")]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return GetAdminView("System Administration");
        }

        [HttpGet("Users")]
        public IActionResult Users()
        {
            return GetAdminView("Manage Users");
        }

        [HttpGet("Roles")]
        public IActionResult Roles()
        {
            return GetAdminView("Manage Roles");
        }

        [HttpGet("Locations")]
        public IActionResult Locations()
        {
            return GetAdminView("Manage Locations");
        }

        [HttpGet("Chairs")]
        public IActionResult Chairs()
        {
            return GetAdminView("Manage Chairs");
        }

        [HttpGet("Doctors")]
        public IActionResult Doctors()
        {
            return GetAdminView("Manage Doctors");
        }

        [HttpGet("Insurance")]
        public IActionResult Insurance()
        {
            return GetAdminView("Manage Insurance");
        }

        [HttpGet("Procedures")]
        public IActionResult Procedures()
        {
            return GetAdminView("Manage Procedures");
        }

        [HttpGet("Configuration")]
        public IActionResult Configuration()
        {
            return GetAdminView("Configuration");
        }

        [HttpGet("Lookup")]
        public IActionResult Lookup()
        {
            return GetAdminView("Master Lookup");
        }

        private IActionResult GetAdminView(string title)
        {
            var metadata = new UIMetadata
            {
                Title = title,
                ModuleName = "Admin",
                Mode = RenderingMode.Template
            };
            return View("Templates/Admin_List", metadata);
        }

        [HttpGet("CreateUser")]
        public IActionResult CreateUser()
        {
            var metadata = new UIMetadata { Title = "Add User", ModuleName = "Admin", Mode = RenderingMode.Template };
            return View("Templates/Admin_UserForm", metadata);
        }

        [HttpGet("CreateProvider")]
        public IActionResult CreateProvider()
        {
            var metadata = new UIMetadata { Title = "Add Provider", ModuleName = "Admin", Mode = RenderingMode.Template };
            return View("Templates/Admin_ProviderForm", metadata);
        }

        [HttpGet("UserDetails/{id}")]
        public IActionResult UserDetails(string id)
        {
            var metadata = new UIMetadata { Title = "User Detail", ModuleName = "Admin", Mode = RenderingMode.Template };
            return View("Templates/Admin_UserDetail", metadata);
        }
    }
}
