using System;
using System.ComponentModel.DataAnnotations;
using Clinic.Domain.Enums;

namespace Clinic.Application.DTOs.MasterData
{
    public class AppointmentTypeCreateDto
    {
        [Required(ErrorMessage = "Code is required.")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public AppointmentComplexity Complexity { get; set; } = AppointmentComplexity.Medium;

        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int DurationInMinutes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
