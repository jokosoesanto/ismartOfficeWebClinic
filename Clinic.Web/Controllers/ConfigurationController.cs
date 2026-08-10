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
        private readonly Clinic.Application.Interfaces.MasterData.IMasterReferenceService _masterReferenceService;

        public ConfigurationController(IAppConfigurationService configurationService, Clinic.Application.Interfaces.MasterData.IMasterReferenceService masterReferenceService)
        {
            _configurationService = configurationService;
            _masterReferenceService = masterReferenceService;
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
        [HttpGet("Currency")]
        [Authorize(Policy = "Configuration.Currency")]
        public async Task<IActionResult> Currency()
        {
            var currentCurrency = await _configurationService.GetValueAsync("ApplicationCurrency", "IDR");
            var currencies = await _masterReferenceService.GetByCategoryAsync("Currency");

            ViewBag.CurrentCurrency = currentCurrency;
            ViewBag.Currencies = System.Linq.Enumerable.ToList(currencies);

            return View();
        }

        [HttpPost("UpdateCurrency")]
        [Authorize(Policy = "Configuration.UpdateCurrency")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCurrency(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                TempData["ErrorMessage"] = "Currency Code is required.";
                return RedirectToAction(nameof(Currency));
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = null;
            if (Guid.TryParse(userIdString, out Guid parsedId)) userId = parsedId;

            var dto = new AppConfigurationDto
            {
                Category = "System",
                Key = "ApplicationCurrency",
                Value = currencyCode.ToUpperInvariant(),
                Description = "Application-wide currency format"
            };

            await _configurationService.UpdateAsync(dto, userId);
            TempData["SuccessMessage"] = "Application currency updated successfully.";
            return RedirectToAction(nameof(Currency));
        }
    }
}