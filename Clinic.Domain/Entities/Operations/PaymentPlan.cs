using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.Operations
{
    public class PaymentPlan
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        
        public DateTime AgreementDate { get; set; } = DateTime.UtcNow;
        public decimal TotalPlannedAmount { get; set; }
        public string Status { get; set; } = "Active"; 
        
        public ICollection<PaymentPlanInstallment> Installments { get; set; } = new List<PaymentPlanInstallment>();
        
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
