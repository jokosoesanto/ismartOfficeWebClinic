using System;

namespace Clinic.Domain.Entities.MasterData
{
    public class PatientGuarantor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        
        public string Name { get; set; } = null!;
        public string? Relationship { get; set; } // Reference Category: Relationship
        
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        
        public bool IsPrimary { get; set; }
        
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
