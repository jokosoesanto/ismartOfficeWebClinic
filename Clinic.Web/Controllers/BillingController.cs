using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.UI;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class BillingController : Controller
    {
        private readonly Clinic.Application.Interfaces.Operations.IInvoiceService _invoiceService;
        private readonly Clinic.Application.Interfaces.Operations.IPaymentService _paymentService;
        private readonly Clinic.Application.Interfaces.MasterData.IMasterReferenceService _masterReferenceService;
        private readonly Clinic.Application.Interfaces.Operations.IAppointmentTreatmentService _appointmentTreatmentService;
        private readonly Clinic.Application.Interfaces.Operations.IAppointmentService _appointmentService;

        public BillingController(
            Clinic.Application.Interfaces.Operations.IInvoiceService invoiceService,
            Clinic.Application.Interfaces.Operations.IPaymentService paymentService,
            Clinic.Application.Interfaces.MasterData.IMasterReferenceService masterReferenceService,
            Clinic.Application.Interfaces.Operations.IAppointmentTreatmentService appointmentTreatmentService,
            Clinic.Application.Interfaces.Operations.IAppointmentService appointmentService)
        {
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _masterReferenceService = masterReferenceService;
            _appointmentTreatmentService = appointmentTreatmentService;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return View("Index", invoices);
        }

        [HttpGet("Payment")]
        public IActionResult Payment()
        {
            var metadata = new UIMetadata { Title = "Process Payment", ModuleName = "Billing", Mode = RenderingMode.Template };
            return View("Templates/Payment_Form", metadata);
        }

        [HttpGet("History")]
        public async Task<IActionResult> History()
        {
            var metadata = new UIMetadata { Title = "Payment History", ModuleName = "Billing", Mode = RenderingMode.Template };
            ViewBag.Metadata = metadata;
            
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return View("Templates/Payment_History", invoices);
        }

        [HttpGet("Preview/{id}")]
        public IActionResult Preview(string id)
        {
            var metadata = new UIMetadata { Title = "Receipt Preview", ModuleName = "Billing", Mode = RenderingMode.Template };
            return View("Templates/Payment_Preview", metadata);
        }

        [HttpGet("Appointment/{appointmentId:guid}/CheckoutReview")]
        public async Task<IActionResult> CheckoutReview(Guid appointmentId)
        {
            var appointment = await _appointmentService.GetByIdAsync(appointmentId);
            if (appointment == null) return NotFound();

            var existingInvoice = await _invoiceService.GetInvoiceByAppointmentIdAsync(appointmentId);
            if (existingInvoice != null)
            {
                TempData["ErrorMessage"] = "Invoice already generated for this appointment.";
                return RedirectToAction("AppointmentInvoice", new { appointmentId });
            }

            var treatments = await _appointmentTreatmentService.GetTreatmentsByAppointmentIdAsync(appointmentId);
            treatments = treatments.Where(t => t.Status == Clinic.Domain.Enums.TreatmentStatus.Executed).ToList();
            
            ViewBag.Appointment = appointment;
            ViewBag.Treatments = treatments;
            
            var metadata = new UIMetadata { Title = "Checkout Review", ModuleName = "Billing", Mode = RenderingMode.Template };
            ViewBag.Metadata = metadata;

            return View("Templates/CheckoutReview", treatments);
        }

        [HttpPost("Appointment/{appointmentId:guid}/CheckoutReview")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutReviewPost(Guid appointmentId, [FromForm] Dictionary<Guid, string> actualPrices)
        {
            var appointment = await _appointmentService.GetByIdAsync(appointmentId);
            if (appointment == null) return NotFound();

            var parsedPrices = new Dictionary<Guid, decimal>();
            if (actualPrices != null)
            {
                foreach (var kvp in actualPrices)
                {
                    // W3C <input type="number"> submits values using '.' as the decimal separator.
                    // By capturing raw string and parsing with InvariantCulture, we bypass OS locale-dependent
                    // thousands-separator inflation (e.g., 850.00 -> 85000 in id-ID).
                    if (decimal.TryParse(kvp.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedValue))
                    {
                        parsedPrices[kvp.Key] = parsedValue;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid price format received.";
                        return RedirectToAction("CheckoutReview", new { appointmentId });
                    }
                }
            }

            var result = await _appointmentTreatmentService.UpdateFinancialsAsync(appointmentId, parsedPrices);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("CheckoutReview", new { appointmentId });
            }

            return RedirectToAction("AppointmentInvoice", new { appointmentId });
        }

        [HttpGet("Appointment/{appointmentId:guid}/Invoice")]
        public async Task<IActionResult> AppointmentInvoice(Guid appointmentId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                var invoice = await _invoiceService.GenerateInvoiceAsync(appointmentId, userId);
                
                var payments = await _paymentService.GetPaymentsByInvoiceIdAsync(invoice.Id);
                decimal totalPaid = 0;
                foreach(var p in payments) { totalPaid += p.Amount; }
                
                ViewBag.Payments = payments;
                ViewBag.TotalPaid = totalPaid;
                ViewBag.Outstanding = invoice.TotalAmount - totalPaid;
                
                string paymentStatus = "Unpaid";
                if (totalPaid >= invoice.TotalAmount)
                    paymentStatus = "Paid";
                else if (totalPaid > 0)
                    paymentStatus = "Partially Paid";
                
                ViewBag.PaymentStatus = paymentStatus;

                ViewBag.PaymentMethods = await _masterReferenceService.GetByCategoryAsync("PaymentMethod");
                
                return View("Invoice", invoice);
            }
            catch (InvalidOperationException ex)
            {
                // Fallback for empty treatment or invalid appointment
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Appointment");
            }
        }

        [HttpPost("Appointment/{appointmentId:guid}/Invoice/Payment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPayment(Guid appointmentId, Guid invoiceId, decimal amount, string paymentMethod, string? referenceNumber, string? notes)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                await _paymentService.CreatePaymentAsync(invoiceId, amount, paymentMethod, referenceNumber, notes, userId);
                TempData["SuccessMessage"] = "Payment recorded successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("AppointmentInvoice", new { appointmentId = appointmentId });
        }
    }
}
