using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("Report/[controller]")]
    public class AgingReportController : Controller
    {
        private readonly Clinic.Infrastructure.Data.AppDbContext _dbContext;

        public AgingReportController(Clinic.Infrastructure.Data.AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;

            // Follow the established formula in HomeController (Global Receivables)
            var query = _dbContext.Invoices
                .AsNoTracking()
                .Include(i => i.Appointment)
                .ThenInclude(a => a!.Patient)
                .Where(i => !i.IsDeleted && i.Status == "Finalized")
                .Select(i => new
                {
                    Invoice = i,
                    Patient = i.Appointment!.Patient,
                    OutstandingBalance = i.TotalAmount
                        - i.Adjustments.Where(a => !a.IsDeleted).Sum(a => (decimal?)a.Amount ?? 0m)
                        - i.Payments.Where(p => !p.IsDeleted).Sum(p => (decimal?)p.Amount ?? 0m)
                        + i.Payments.Where(p => !p.IsDeleted).SelectMany(p => p.Refunds).Where(r => !r.IsDeleted).Sum(r => (decimal?)r.Amount ?? 0m)
                })
                .Where(i => i.OutstandingBalance > 0); // Only open receivables

            var openInvoices = await query.ToListAsync();

            var model = new AgingReportViewModel
            {
                TotalOutstanding = openInvoices.Sum(x => x.OutstandingBalance),
                TotalInvoices = openInvoices.Count,
                AgingDetails = new List<AgingInvoiceViewModel>()
            };

            foreach (var item in openInvoices)
            {
                var daysOverdue = (int)(today - item.Invoice.InvoiceDate.Date).TotalDays;
                
                string bucket;
                if (daysOverdue <= 0) bucket = "Current";
                else if (daysOverdue <= 30) bucket = "1-30 Days";
                else if (daysOverdue <= 60) bucket = "31-60 Days";
                else if (daysOverdue <= 90) bucket = "61-90 Days";
                else bucket = "Over 90 Days";

                model.AgingDetails.Add(new AgingInvoiceViewModel
                {
                    InvoiceId = item.Invoice.Id,
                    InvoiceNumber = item.Invoice.InvoiceNumber,
                    PatientName = item.Patient?.FullName ?? "Unknown",
                    PatientId = item.Patient?.Id,
                    InvoiceDate = item.Invoice.InvoiceDate,
                    DaysOverdue = daysOverdue,
                    Bucket = bucket,
                    OutstandingBalance = item.OutstandingBalance
                });

                if (bucket == "Current") model.CurrentTotal += item.OutstandingBalance;
                else if (bucket == "1-30 Days") model.Days1To30Total += item.OutstandingBalance;
                else if (bucket == "31-60 Days") model.Days31To60Total += item.OutstandingBalance;
                else if (bucket == "61-90 Days") model.Days61To90Total += item.OutstandingBalance;
                else if (bucket == "Over 90 Days") model.Over90DaysTotal += item.OutstandingBalance;
            }

            model.AgingDetails = model.AgingDetails
                .OrderByDescending(x => x.DaysOverdue)
                .ThenBy(x => x.InvoiceDate)
                .ToList();

            return View(model);
        }
    }

    public class AgingReportViewModel
    {
        public decimal TotalOutstanding { get; set; }
        public int TotalInvoices { get; set; }

        public decimal CurrentTotal { get; set; }
        public decimal Days1To30Total { get; set; }
        public decimal Days31To60Total { get; set; }
        public decimal Days61To90Total { get; set; }
        public decimal Over90DaysTotal { get; set; }

        public List<AgingInvoiceViewModel> AgingDetails { get; set; } = new List<AgingInvoiceViewModel>();
    }

    public class AgingInvoiceViewModel
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public string PatientName { get; set; } = "";
        public Guid? PatientId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int DaysOverdue { get; set; }
        public string Bucket { get; set; } = "";
        public decimal OutstandingBalance { get; set; }
    }
}
