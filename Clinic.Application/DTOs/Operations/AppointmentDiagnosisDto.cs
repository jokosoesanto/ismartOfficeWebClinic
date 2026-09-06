using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.Operations
{
    public class AppointmentDiagnosisDto
    {
        public Guid? Id { get; set; }
        
        [Required]
        public Guid AppointmentId { get; set; }
        
        [Required]
        public Guid DiagnosisMasterId { get; set; }
        
        public string? DiagnosisCode { get; set; }
        public string? DiagnosisName { get; set; }
        
        [StringLength(1000)]
        public string? Remark { get; set; }
    }
}
