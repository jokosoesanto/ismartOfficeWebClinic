$controllers = @(
    @{ Name = "Appointment"; Title = "Appointments"; Template = "Templates/Scheduler"; Component = "PrototypeList" },
    @{ Name = "MedicalRecord"; Title = "Medical Records"; Template = "Templates/MedicalRecord"; Component = "PrototypeMasterDetail" },
    @{ Name = "Billing"; Title = "Billing & Payment"; Template = "Templates/TransactionList"; Component = "PrototypeTransaction" },
    @{ Name = "Inventory"; Title = "Inventory"; Template = "Templates/MasterList"; Component = "PrototypeList" },
    @{ Name = "Report"; Title = "Reports"; Template = "Templates/ReportViewer"; Component = "PrototypeReport" }
)

$csTemplate = @"
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    [Route("[controller]")]
    public class __NAME__Controller : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var metadata = new UIMetadata
            {
                Title = "__TITLE__",
                ModuleName = "__NAME__",
                Composition = new UIComposition
                {
                    Center = new List<UIComponent>
                    {
                        new UIComponent { ComponentId = "__COMPONENT__" }
                    }
                }
            };
            return View("__TEMPLATE__", metadata);
        }
    }
}
"@

$adminTemplate = @"
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

        private IActionResult GetAdminView(string title)
        {
            var metadata = new UIMetadata
            {
                Title = title,
                ModuleName = "Admin",
                Composition = new UIComposition
                {
                    Center = new List<UIComponent>
                    {
                        new UIComponent { ComponentId = "PrototypeAdministration" }
                    }
                }
            };
            return View("Templates/MasterList", metadata);
        }
    }
}
"@

$baseDir = "C:\Users\cipac\Documents\Projects\ismartOfficeWebClinic\Clinic.Web\Controllers"

foreach ($c in $controllers) {
    $csFile = "$baseDir\$($c.Name)Controller.cs"
    $csContent = $csTemplate.Replace("__NAME__", $c.Name).Replace("__TITLE__", $c.Title).Replace("__TEMPLATE__", $c.Template).Replace("__COMPONENT__", $c.Component)
    Set-Content -Path $csFile -Value $csContent
}

Set-Content -Path "$baseDir\AdminController.cs" -Value $adminTemplate

Write-Host "All controllers generated successfully."
