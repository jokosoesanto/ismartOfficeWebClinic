using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Domain.Entities.MasterData
{
    public class PatientMedicalHistory
    {
        public Guid Id { get; set; } = Guid.Empty; // using Guid.Empty so EF treats as Added
        
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Condition { get; set; } = null!;
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        // Active/Historical state (true = Active, false = Resolved/Historical)
        public bool IsActive { get; set; } = true;

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
