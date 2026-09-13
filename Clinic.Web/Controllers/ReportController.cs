using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Infrastructure.Data;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Web.Models;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ReportController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrencyService _currencyService;

        public ReportController(AppDbContext dbContext, ICurrencyService currencyService)
        {
            _dbContext = dbContext;
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            var metadata = new UIMetadata
            {
                Title = "Collection Report",
                ModuleName = "Report",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = metadata;

            // Date processing
            var today = DateTime.Today;
            var from = fromDate?.Date ?? today;
            var to = toDate?.Date ?? today;

            // Ensure From is not greater than To
            if (from > to)
            {
                from = to;
            }

            var nextDay = to.AddDays(1);

            // Fetch payments safely using PaymentDate
            var payments = await _dbContext.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Appointment)
                .ThenInclude(a => a!.Patient)
                .Where(p => !p.IsDeleted && p.PaymentDate >= from && p.PaymentDate < nextDay)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            var totalCollection = payments.Sum(p => p.Amount);
            var formattedTotal = await _currencyService.FormatAmountAsync(totalCollection);

            // Format amounts for each payment
            foreach (var payment in payments)
            {
                ViewData[$"FormattedAmount_{payment.Id}"] = await _currencyService.FormatAmountAsync(payment.Amount);
            }

            var viewModel = new CollectionReportViewModel
            {
                FromDate = from,
                ToDate = to,
                TotalCollection = totalCollection,
                FormattedTotalCollection = formattedTotal,
                Payments = payments
            };

            return View("Templates/ReportViewer", viewModel);
        }
    }
}
