using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Domain.Entities.Operations
{
    public class AppointmentChiefComplaint
    {
        // Using Guid.Empty by default so EF Core correctly tracks this as 'Added' (for the Guid issue)
        public Guid Id { get; set; } = Guid.Empty;

        // Foreign Key
        public Guid AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        [Required]
        [MaxLength(200)]
        public string Complaint { get; set; } = null!;

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(10)]
        public string? ToothNumber { get; set; }

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
