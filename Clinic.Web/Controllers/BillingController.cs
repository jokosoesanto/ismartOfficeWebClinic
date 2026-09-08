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

        public BillingController(
            Clinic.Application.Interfaces.Operations.IInvoiceService invoiceService,
            Clinic.Application.Interfaces.Operations.IPaymentService paymentService,
            Clinic.Application.Interfaces.MasterData.IMasterReferenceService masterReferenceService)
        {
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _masterReferenceService = masterReferenceService;
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
        public IActionResult History()
        {
            var metadata = new UIMetadata { Title = "Payment History", ModuleName = "Billing", Mode = RenderingMode.Template };
            return View("Templates/Payment_History", metadata);
        }

        [HttpGet("Preview/{id}")]
        public IActionResult Preview(string id)
        {
            var metadata = new UIMetadata { Title = "Receipt Preview", ModuleName = "Billing", Mode = RenderingMode.Template };
            return View("Templates/Payment_Preview", metadata);
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
