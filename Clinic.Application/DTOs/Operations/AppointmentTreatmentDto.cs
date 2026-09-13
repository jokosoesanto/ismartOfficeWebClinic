using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.Operations
{
    public class AppointmentTreatmentDto
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid AppointmentId { get; set; }
        
        [Required]
        public Guid TreatmentItemId { get; set; }
        
        public string? TreatmentItemName { get; set; }
        public string? TreatmentItemColor { get; set; }
        
        public string? SiteNumber { get; set; }
        
        public string? SiteDetail { get; set; }
        
        [Required]
        [Range(0, 999999999.99, ErrorMessage = "Price must be a positive value")]
        public decimal ActualPrice { get; set; }
        public string? Remark { get; set; }
        public Clinic.Domain.Enums.TreatmentStatus Status { get; set; }
        public bool HasConsent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
