using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Domain.Entities.Operations
{
    public class PrescriptionItem
    {
        public Guid Id { get; set; }
        
        public Guid PrescriptionId { get; set; }
        public Prescription? Prescription { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string MedicationName { get; set; } = null!;
        
        [Required]
        [MaxLength(100)]
        public string Dosage { get; set; } = null!;
        
        [Required]
        [MaxLength(100)]
        public string Frequency { get; set; } = null!;
        
        [Required]
        [MaxLength(200)]
        public string Instruction { get; set; } = null!;

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
