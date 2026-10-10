using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Clinic.Domain.Enums;

namespace Clinic.Application.DTOs.Operations
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Appointment Type is required")]
        public Guid AppointmentTypeId { get; set; }
        
        public string? AppointmentTypeName { get; set; }
        public Clinic.Domain.Enums.AppointmentComplexity? AppointmentComplexity { get; set; }

        public Guid? TreatmentItemId { get; set; }
        public string? TreatmentItemName { get; set; }

        [Required(ErrorMessage = "Patient is required")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Doctor is required")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public Guid LocationId { get; set; }

        [Required(ErrorMessage = "Chair is required")]
        public Guid ChairId { get; set; }

        public Guid? AssistantId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Start Time is required")]
        public TimeSpan StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Schedule;

        public string? Notes { get; set; }

        // Read-only properties for display
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? LocationName { get; set; }
        public string? ChairName { get; set; }
        public string? AssistantName { get; set; }

        public bool IsDeleted { get; set; }
        
        public IEnumerable<AppointmentChiefComplaintDto> ChiefComplaints { get; set; } = new List<AppointmentChiefComplaintDto>();
        
        public AppointmentVitalSignDto? VitalSign { get; set; }

        public AppointmentClinicalNoteDto? ClinicalNote { get; set; }
    }
}
