using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Clinic.Web.Services.Diagnostics;

namespace Clinic.Web.Controllers
{
    [Route("diagnostics/[controller]")]
    public class DiagnosticsController : Controller
    {
        private readonly IComponentDiagnosticsService _diagnosticsService;
        private readonly IRegistryValidatorService _validatorService;

        public DiagnosticsController(IComponentDiagnosticsService diagnosticsService, IRegistryValidatorService validatorService)
        {
            _diagnosticsService = diagnosticsService;
            _validatorService = validatorService;
        }

        [HttpGet("components")]
        public IActionResult Components()
        {
            var manifest = _diagnosticsService.GetAllComponents().ToList();
            return View("~/Views/Diagnostics/Components.cshtml", manifest);
        }

        [HttpGet("validation")]
        public IActionResult Validation()
        {
            var report = _validatorService.GenerateReport();
            return View("~/Views/Diagnostics/Validation.cshtml", report);
        }
    }
}
