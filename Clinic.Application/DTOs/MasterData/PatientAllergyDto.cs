using System;

namespace Clinic.Application.DTOs.MasterData
{
    public class PatientAllergyDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string Allergen { get; set; } = string.Empty;
        public string? Severity { get; set; }
        public string? Notes { get; set; }
    }
}
