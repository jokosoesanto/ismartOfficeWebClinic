using System;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Domain.Entities.Operations
{
    public class AppointmentVitalSign
    {
        // Must use Guid.Empty initially to ensure proper EntityState.Added tracking
        public Guid Id { get; set; } = Guid.Empty;

        // Foreign Keys
        public Guid AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public int? Systolic { get; set; }
        public int? Diastolic { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Temperature { get; set; }

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
