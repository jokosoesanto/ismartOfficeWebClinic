using System;

namespace Clinic.Domain.Entities.Operations
{
    public class InvoiceLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        
        public Guid AppointmentTreatmentId { get; set; }
        public AppointmentTreatment? AppointmentTreatment { get; set; }
        
        public string Description { get; set; } = null!;
        
        public int Quantity { get; set; } = 1;
        
        public decimal UnitPrice { get; set; }
        
        public decimal LineTotal { get; set; }
        
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
