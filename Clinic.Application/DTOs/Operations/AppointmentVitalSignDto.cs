using System;

namespace Clinic.Application.DTOs.Operations
{
    public class AppointmentVitalSignDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        
        public int? Systolic { get; set; }
        public int? Diastolic { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Temperature { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
