using System;

namespace Clinic.Domain.Entities.Operations
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        
        public string ReceiptNumber { get; set; } = null!;
        
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        
        public decimal Amount { get; set; }
        
        public string PaymentMethod { get; set; } = null!;
        
        public string? ReferenceNumber { get; set; }
        
        public string? Notes { get; set; }
        
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
