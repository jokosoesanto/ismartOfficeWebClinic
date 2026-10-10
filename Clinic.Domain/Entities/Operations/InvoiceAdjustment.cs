using System;

namespace Clinic.Domain.Entities.Operations
{
    public class InvoiceAdjustment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        
        public string AdjustmentType { get; set; } = "Credit";
        
        public decimal Amount { get; set; }
        
        public DateTime AdjustmentDate { get; set; } = DateTime.UtcNow;
        
        public string? Reason { get; set; }
        
        // Audit fields
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
