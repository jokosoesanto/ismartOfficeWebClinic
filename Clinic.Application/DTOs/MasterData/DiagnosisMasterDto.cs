using System;

namespace Clinic.Application.DTOs.MasterData
{
    public class DiagnosisMasterDto
    {
        public Guid Id { get; set; }
        public string DiagnosisCode { get; set; } = string.Empty;
        public string DiagnosisName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
