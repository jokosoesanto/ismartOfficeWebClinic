using System;
using System.Collections.Generic;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Web.Models
{
    public class CollectionReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalCollection { get; set; }
        public string FormattedTotalCollection { get; set; } = string.Empty;
        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}
