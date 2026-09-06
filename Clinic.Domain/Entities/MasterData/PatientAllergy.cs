using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Domain.Entities.MasterData
{
    public class PatientAllergy
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Allergen { get; set; } = null!;
        
        [MaxLength(50)]
        public string? Severity { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }

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
