using System;

namespace Clinic.Domain.Entities.Operations
{
    public class PatientCredit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid PatientId { get; set; }
        public Clinic.Domain.Entities.MasterData.Patient? Patient { get; set; }
        
        public decimal Amount { get; set; }
        
        public DateTime CreditDate { get; set; } = DateTime.UtcNow;
        
        public string? Reason { get; set; }
        
        // Audit & Soft Delete
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
