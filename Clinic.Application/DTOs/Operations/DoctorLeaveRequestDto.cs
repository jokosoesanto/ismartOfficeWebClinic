using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.Operations
{
    public class DoctorLeaveRequestDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Doctor is required")]
        public Guid DoctorId { get; set; }

        public string? Reason { get; set; }

        /// <summary>
        /// The list of selected leave dates.
        /// </summary>
        public List<DateTime> LeaveDates { get; set; } = new List<DateTime>();

        // Read-only display properties
        public string? DoctorName { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Detailed information about the individual dates, including cancellation status.
        /// </summary>
        public List<DoctorLeaveDateDto> LeaveDateDetails { get; set; } = new List<DoctorLeaveDateDto>();
    }
}
