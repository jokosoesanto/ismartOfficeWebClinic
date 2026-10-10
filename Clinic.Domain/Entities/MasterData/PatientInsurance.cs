using System;

namespace Clinic.Domain.Entities.MasterData
{
    public class PatientInsurance
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        
        public Guid InsuranceId { get; set; }
        public Insurance Insurance { get; set; } = null!;
        
        public string? MemberId { get; set; }
        public string? GroupId { get; set; }
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
