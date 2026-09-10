using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Domain.Entities.MasterData
{
    public class PatientMedication
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string MedicationName { get; set; } = null!;
        
        [MaxLength(200)]
        public string? Dosage { get; set; }
        
        public DateTime? PrescribedDate { get; set; }
        
        // Active/Historical state (true = Active, false = Completed/Historical)
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
