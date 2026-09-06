using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Domain.Entities.MasterData
{
    public class DiagnosisMaster
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string DiagnosisCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string DiagnosisName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        
        public bool IsDeleted { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
