using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Clinic.Application.Interfaces.Operations;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("MedicalRecord/PatientCredit")]
    public class PatientCreditController : Controller
    {
        private readonly IPatientCreditService _patientCreditService;

        public PatientCreditController(IPatientCreditService patientCreditService)
        {
            _patientCreditService = patientCreditService;
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid patientId, decimal amount, string? reason)
        {
            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Prepayment amount must be greater than zero.";
                return Redirect($"/MedicalRecord/Chart/{patientId}#ledger");
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdString, out Guid userId);

            try
            {
                await _patientCreditService.CreateCreditAsync(patientId, amount, reason, userId);
                TempData["SuccessMessage"] = "Patient credit has been added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to add credit: {ex.Message}";
            }

            return Redirect($"/MedicalRecord/Chart/{patientId}#ledger");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid patientId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdString, out Guid userId);

            try
            {
                await _patientCreditService.DeleteCreditAsync(id, userId);
                TempData["SuccessMessage"] = "Patient credit has been deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to delete credit: {ex.Message}";
            }

            return Redirect($"/MedicalRecord/Chart/{patientId}#ledger");
        }
    }
}
