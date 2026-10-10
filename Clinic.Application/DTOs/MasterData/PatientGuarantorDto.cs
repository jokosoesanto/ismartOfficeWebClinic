using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class PatientGuarantorDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;
        
        public string? Relationship { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        
        public bool IsPrimary { get; set; }
    }
}
