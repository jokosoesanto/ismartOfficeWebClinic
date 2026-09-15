using System;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Domain.Entities.Operations
{
    public class AppointmentFollowUp
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
        
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }
        
        public bool IsFollowUpRequired { get; set; }
        public DateTime? RecommendedDate { get; set; }
        public string? Reason { get; set; }
        
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
