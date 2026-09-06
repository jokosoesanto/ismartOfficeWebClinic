using System;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Domain.Entities.Operations
{
    public class AppointmentClinicalNote
    {
        // Must use Guid.Empty initially to ensure proper EntityState.Added tracking
        public Guid Id { get; set; } = Guid.Empty;

        // Foreign Keys
        public Guid AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public string? Subjective { get; set; }
        public string? Objective { get; set; }
        public string? Assessment { get; set; }
        public string? Plan { get; set; }

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
