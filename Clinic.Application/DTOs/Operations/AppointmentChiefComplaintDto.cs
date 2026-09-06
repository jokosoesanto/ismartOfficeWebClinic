using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.Operations
{
    public class AppointmentChiefComplaintDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }

        [Required(ErrorMessage = "Complaint is required")]
        [StringLength(200)]
        public string Complaint { get; set; } = null!;

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(10)]
        public string? ToothNumber { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}
