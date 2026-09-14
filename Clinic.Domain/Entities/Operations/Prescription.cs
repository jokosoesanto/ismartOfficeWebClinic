using System;
using System.Collections.Generic;
using Clinic.Domain.Entities.MasterData;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Domain.Entities.Operations
{
    public class Prescription
    {
        public Guid Id { get; set; }
        
        public Guid AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
        
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }
        
        public Guid PrescribedByUserId { get; set; }
        public User? PrescribedByUser { get; set; }
        
        public DateTime PrescribedAt { get; set; } = DateTime.UtcNow;
        
        public string Status { get; set; } = "Active";

        public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();

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
