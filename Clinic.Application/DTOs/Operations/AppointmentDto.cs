using System;
using System.ComponentModel.DataAnnotations;
using Clinic.Domain.Enums;

namespace Clinic.Application.DTOs.Operations
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Patient is required")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Doctor is required")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public Guid LocationId { get; set; }

        [Required(ErrorMessage = "Chair is required")]
        public Guid ChairId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Start Time is required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End Time is required")]
        public TimeSpan EndTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Schedule;

        public string? Notes { get; set; }

        // Read-only properties for display
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? LocationName { get; set; }
        public string? ChairName { get; set; }
    }
}
