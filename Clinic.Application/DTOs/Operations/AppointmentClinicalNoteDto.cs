using System;

namespace Clinic.Application.DTOs.Operations
{
    public class AppointmentClinicalNoteDto
    {
        public Guid AppointmentId { get; set; }
        public string? Subjective { get; set; }
        public string? Objective { get; set; }
        public string? Assessment { get; set; }
        public string? Plan { get; set; }
    }
}
