using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Clinic.Infrastructure.Data;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Web.Models;

namespace Clinic.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrencyService _currencyService;

        public HomeController(AppDbContext dbContext, ICurrencyService currencyService)
        {
            _dbContext = dbContext;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var metadata = new UIMetadata
            {
                Title = "Dashboard",
                ModuleName = "Dashboard",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = metadata;

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // 1. Today's Schedule (All appointments for today)
            var todaysAppointments = await _dbContext.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.Date >= today && a.Date < tomorrow)
                .ToListAsync();

            todaysAppointments = todaysAppointments
                .OrderBy(a => a.StartTime)
                .ToList();

            // 2. Waiting Patients (Checked-in / OnTime)
            var waitingPatients = todaysAppointments
                .Where(a => a.Status == Clinic.Domain.Enums.AppointmentStatus.OnTime)
                .ToList();

            // 3. Today's Collection & Recent Payments
            var todaysPayments = await _dbContext.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Appointment)
                .ThenInclude(a => a!.Patient)
                .Where(p => p.PaymentDate >= today && p.PaymentDate < tomorrow && !p.IsDeleted)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            var todaysCollection = todaysPayments.Sum(p => p.Amount);
            var formattedCollection = await _currencyService.FormatAmountAsync(todaysCollection);

            var viewModel = new DashboardViewModel
            {
                TodaysAppointments = todaysAppointments,
                WaitingPatients = waitingPatients,
                TodaysCollection = todaysCollection,
                FormattedTodaysCollection = formattedCollection,
                RecentPayments = todaysPayments.Take(5).ToList()
            };

            return View("Dashboard", viewModel);
        }
    }
}
