using System;

namespace Clinic.Domain.Entities.Operations
{
    public class PaymentAllocation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid PaymentId { get; set; }
        public Payment? Payment { get; set; }
        
        public Guid InvoiceLineId { get; set; }
        public InvoiceLine? InvoiceLine { get; set; }
        
        public decimal AllocatedAmount { get; set; }
        
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
