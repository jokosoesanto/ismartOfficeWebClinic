using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.Operations
{
    public class Invoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string InvoiceNumber { get; set; } = null!;
        
        public Guid AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
        
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        
        public string Status { get; set; } = "Finalized"; 
        
        public decimal TotalAmount { get; set; }
        
        public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
        
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        
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
