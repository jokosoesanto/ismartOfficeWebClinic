using System;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Domain.Entities.Operations
{
    public class AppointmentTreatment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign Keys
        public Guid AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public Guid TreatmentItemId { get; set; }
        public TreatmentCatalog? TreatmentItem { get; set; }

        // Data fields based on Desktop parity
        public string? SiteNumber { get; set; }
        public string? SiteDetail { get; set; }
        public decimal ActualPrice { get; set; }
        public string? Remark { get; set; }

        public Clinic.Domain.Enums.TreatmentStatus Status { get; set; } = Clinic.Domain.Enums.TreatmentStatus.Executed;

        // Consent
        public TreatmentConsent? Consent { get; set; }

        // Audit fields (required by entity framework tracking standards for consistency)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
