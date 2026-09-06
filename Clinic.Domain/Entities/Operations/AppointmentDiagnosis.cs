using System;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Domain.Entities.Operations
{
    public class AppointmentDiagnosis
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid DiagnosisMasterId { get; set; }
        public string? Remark { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation Properties
        public Appointment? Appointment { get; set; }
        public DiagnosisMaster? DiagnosisMaster { get; set; }
    }
}
