using System;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Domain.Entities.Operations
{
    public class TreatmentConsent
    {
        public Guid Id { get; set; }

        public Guid AppointmentTreatmentId { get; set; }
        public AppointmentTreatment? AppointmentTreatment { get; set; }

        public bool IsConsentGiven { get; set; } = true;

        public DateTime ConsentedAt { get; set; } = DateTime.UtcNow;
        public Guid ConsentedByUserId { get; set; }
        public User? ConsentedByUser { get; set; }
    }
}