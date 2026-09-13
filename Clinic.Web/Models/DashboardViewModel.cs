using System;
using System.Collections.Generic;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Web.Models
{
    public class DashboardViewModel
    {
        public List<Appointment> TodaysAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> WaitingPatients { get; set; } = new List<Appointment>();
        public decimal TodaysCollection { get; set; }
        public string FormattedTodaysCollection { get; set; } = "$0.00";
        public decimal OutstandingReceivables { get; set; }
        public string FormattedOutstandingReceivables { get; set; } = "$0.00";
        public List<Payment> RecentPayments { get; set; } = new List<Payment>();
    }
}
