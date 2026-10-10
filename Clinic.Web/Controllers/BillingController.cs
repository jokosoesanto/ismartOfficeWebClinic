using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.UI;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        private readonly Clinic.Application.Interfaces.Operations.IPaymentRepository _paymentRepository;
        private readonly Clinic.Application.Interfaces.Operations.IInvoiceRepository _invoiceRepository;

        public BillingController(
            Clinic.Application.Interfaces.Operations.IInvoiceService invoiceService,
            Clinic.Application.Interfaces.Operations.IPaymentService paymentService,
            Clinic.Application.Interfaces.MasterData.IMasterReferenceService masterReferenceService,
            Clinic.Application.Interfaces.Operations.IAppointmentTreatmentService appointmentTreatmentService,
            Clinic.Application.Interfaces.Operations.IAppointmentService appointmentService,
            Clinic.Application.Interfaces.Operations.IPaymentRepository paymentRepository,
            Clinic.Application.Interfaces.Operations.IInvoiceRepository invoiceRepository)
        {
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _masterReferenceService = masterReferenceService;
            _appointmentTreatmentService = appointmentTreatmentService;
            _appointmentService = appointmentService;
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
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

        [HttpGet("Payment/{paymentId:guid}/Allocate")]
        public async Task<IActionResult> AllocatePayment(Guid paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) return NotFound();

            var invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceId);
            if (invoice == null) return NotFound();

            var paymentTotalAllocated = await _paymentRepository.GetTotalAllocatedByPaymentIdAsync(paymentId);
            
            var dto = new Clinic.Application.DTOs.Operations.PaymentAllocationDetailsDto
            {
                PaymentId = payment.Id,
                InvoiceId = invoice.Id,
                PaymentAmount = payment.Amount,
                TotalAllocated = paymentTotalAllocated
            };

            var existingAllocations = await _paymentRepository.GetAllocationsByPaymentIdAsync(paymentId);

            foreach (var line in invoice.InvoiceLines.Where(il => !il.IsDeleted))
            {
                var lineTotalAllocated = await _paymentRepository.GetTotalAllocatedByInvoiceLineIdAsync(line.Id);

                dto.InvoiceLines.Add(new Clinic.Application.DTOs.Operations.InvoiceLineAllocationDto
                {
                    InvoiceLineId = line.Id,
                    Description = line.Description,
                    LineTotal = line.LineTotal,
                    AlreadyAllocated = lineTotalAllocated,
                    CurrentAllocation = 0 // Initial value for the form
                });
            }

            var metadata = new UIMetadata { Title = "Allocate Payment", ModuleName = "Billing", Mode = RenderingMode.Template };
            ViewBag.Metadata = metadata;
            ViewBag.InvoiceNumber = invoice.InvoiceNumber;
            ViewBag.InvoiceTotal = invoice.TotalAmount;
            ViewBag.PaymentReceipt = payment.ReceiptNumber;
            ViewBag.PaymentDate = payment.PaymentDate;
            
            return View("Templates/Payment_Allocate", dto);
        }

        [HttpPost("Payment/{paymentId:guid}/Allocate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocatePaymentPost(Guid paymentId, [FromForm] Clinic.Application.DTOs.Operations.PaymentAllocationDetailsDto model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) userId = Guid.Empty;

            try
            {
                foreach (var line in model.InvoiceLines.Where(l => l.CurrentAllocation > 0))
                {
                    await _paymentService.AllocatePaymentAsync(paymentId, line.InvoiceLineId, line.CurrentAllocation, userId);
                }

                TempData["SuccessMessage"] = "Payment allocated successfully.";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("AllocatePayment", new { paymentId = paymentId });
            }
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
                var adjustments = await _invoiceService.GetAdjustmentsByInvoiceIdAsync(invoice.Id);
                decimal totalPaid = 0;
                foreach(var p in payments) 
                { 
                    var refunded = p.Refunds?.Where(r => !r.IsDeleted).Sum(r => r.Amount) ?? 0m;
                    totalPaid += (p.Amount - refunded); 
                }

                decimal totalAdjustment = 0;
                foreach(var a in adjustments) { totalAdjustment += a.Amount; }
                
                ViewBag.Payments = payments;
                ViewBag.Adjustments = adjustments;
                ViewBag.TotalPaid = totalPaid;
                ViewBag.TotalAdjustment = totalAdjustment;
                
                decimal outstanding = invoice.TotalAmount - totalPaid - totalAdjustment;
                ViewBag.Outstanding = outstanding;
                
                string paymentStatus = "Unpaid";
                if (outstanding <= 0)
                    paymentStatus = "Paid";
                else if (totalPaid > 0 || totalAdjustment > 0)
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

        [HttpPost("Appointment/{appointmentId:guid}/Invoice/Adjustment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdjustment(Guid appointmentId, Guid invoiceId, decimal amount, string adjustmentType, string? reason)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                await _invoiceService.CreateAdjustmentAsync(invoiceId, amount, adjustmentType, reason, userId);
                TempData["SuccessMessage"] = "Adjustment recorded successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("AppointmentInvoice", new { appointmentId = appointmentId });
        }

        [HttpPost("Payment/{paymentId:guid}/Refund")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRefund(Guid paymentId, decimal amount, string? reason)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                await _paymentService.CreateRefundAsync(paymentId, amount, reason, userId);
                TempData["SuccessMessage"] = "Refund recorded successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            // Redirect back to Payment History
            return RedirectToAction("History");
        }

        [HttpGet("Invoice/{invoiceId:guid}/PaymentPlan")]
        public async Task<IActionResult> PaymentPlan(Guid invoiceId, [FromServices] Clinic.Infrastructure.Data.AppDbContext dbContext)
        {
            var invoice = await dbContext.Invoices
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted);

            if (invoice == null) return NotFound();

            var paymentPlan = await dbContext.PaymentPlans
                .Include(p => p.Installments)
                .FirstOrDefaultAsync(p => p.InvoiceId == invoiceId && !p.IsDeleted);

            var payments = await dbContext.Payments
                .Include(p => p.Refunds)
                .Where(p => p.InvoiceId == invoiceId && !p.IsDeleted)
                .ToListAsync();
            var adjustments = await dbContext.InvoiceAdjustments
                .Where(a => a.InvoiceId == invoiceId && !a.IsDeleted)
                .ToListAsync();

            decimal totalPaid = 0;
            foreach(var p in payments) 
            { 
                var refunded = p.Refunds?.Where(r => !r.IsDeleted).Sum(r => r.Amount) ?? 0m;
                totalPaid += (p.Amount - refunded); 
            }
            decimal totalAdjustment = adjustments.Sum(a => a.Amount);
            decimal outstanding = invoice.TotalAmount - totalPaid - totalAdjustment;

            ViewBag.Invoice = invoice;
            ViewBag.Outstanding = outstanding;
            
            return View("PaymentPlan", paymentPlan);
        }

        [HttpPost("Invoice/{invoiceId:guid}/PaymentPlan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePaymentPlan(Guid invoiceId, decimal totalPlannedAmount, int numberOfInstallments, DateTime startDate, [FromServices] Clinic.Infrastructure.Data.AppDbContext dbContext)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdStr, out var uId) ? uId : null;

            if (totalPlannedAmount <= 0 || numberOfInstallments <= 0)
            {
                TempData["ErrorMessage"] = "Invalid amount or installments.";
                return RedirectToAction("PaymentPlan", new { invoiceId = invoiceId });
            }

            var existingPlan = await dbContext.PaymentPlans.FirstOrDefaultAsync(p => p.InvoiceId == invoiceId && !p.IsDeleted);
            if (existingPlan != null)
            {
                TempData["ErrorMessage"] = "A payment plan already exists.";
                return RedirectToAction("PaymentPlan", new { invoiceId = invoiceId });
            }

            var invoice = await dbContext.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null) return NotFound();

            var plan = new Clinic.Domain.Entities.Operations.PaymentPlan
            {
                InvoiceId = invoiceId,
                AgreementDate = DateTime.UtcNow,
                TotalPlannedAmount = totalPlannedAmount,
                CreatedBy = userId
            };

            decimal amountPerInstallment = Math.Round(totalPlannedAmount / numberOfInstallments, 2);
            decimal runningTotal = 0;

            for (int i = 1; i <= numberOfInstallments; i++)
            {
                decimal currentAmount = amountPerInstallment;
                
                if (i == numberOfInstallments)
                {
                    currentAmount = totalPlannedAmount - runningTotal;
                }

                plan.Installments.Add(new Clinic.Domain.Entities.Operations.PaymentPlanInstallment
                {
                    SequenceNumber = i,
                    DueDate = startDate.AddMonths(i - 1),
                    Amount = currentAmount
                });

                runningTotal += currentAmount;
            }

            dbContext.PaymentPlans.Add(plan);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Payment plan created successfully.";
            return RedirectToAction("PaymentPlan", new { invoiceId = invoiceId });
        }
    }
}
