using System;

namespace Clinic.Domain.Entities.Operations
{
    public class PaymentPlanInstallment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid PaymentPlanId { get; set; }
        public PaymentPlan? PaymentPlan { get; set; }
        
        public int SequenceNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Scheduled"; 
    }
}
