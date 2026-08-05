using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Application.DTOs.Configuration;
using System;
using System.Security.Claims;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("Configuration")]
    public class ConfigurationController : Controller
    {
        private readonly IAppConfigurationService _configurationService;

        public ConfigurationController(IAppConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet("Security")]
        [Authorize(Policy = "Configuration.Security")]
        public async Task<IActionResult> Security()
        {
            var timeout = await _configurationService.GetIntValueAsync("SessionTimeoutMinutes", 30);
            
            var metadata = new Clinic.Application.UI.UIMetadata
            {
                Title = "Security Configuration",
                ModuleName = "Admin",
                Mode = Clinic.Application.UI.RenderingMode.Template,
                Data = timeout
            };
            return View("~/Views/Shared/Templates/Admin_List.cshtml", metadata);
        }

        [HttpPost("UpdateSecurity")]
        [Authorize(Policy = "Configuration.UpdateSecurity")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSecurity(int sessionTimeoutMinutes)
        {
            if (sessionTimeoutMinutes < 5 || sessionTimeoutMinutes > 480)
            {
                TempData["ErrorMessage"] = "Session Timeout must be between 5 and 480 minutes.";
                return RedirectToAction(nameof(Security));
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = null;
            if (Guid.TryParse(userIdString, out Guid parsedId)) userId = parsedId;

            var dto = new AppConfigurationDto
            {
                Category = "Security",
                Key = "SessionTimeoutMinutes",
                Value = sessionTimeoutMinutes.ToString(),
                Description = "Application session timeout in minutes"
            };

            await _configurationService.UpdateAsync(dto, userId);
            TempData["SuccessMessage"] = "Security configuration updated successfully.";
            return RedirectToAction(nameof(Security));
        }
    }
}