using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class PatientInsuranceDto
    {
        public Guid? Id { get; set; }
        
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        public Guid InsuranceId { get; set; }
        public string? InsuranceName { get; set; }
        
        public string? MemberId { get; set; }
        public string? GroupId { get; set; }
        public bool IsPrimary { get; set; }
    }
}
