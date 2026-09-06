using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class DiagnosisMasterCreateDto
    {
        [Required(ErrorMessage = "Diagnosis Code is required.")]
        [StringLength(50, ErrorMessage = "Diagnosis Code cannot exceed 50 characters.")]
        public string DiagnosisCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Diagnosis Name is required.")]
        [StringLength(200, ErrorMessage = "Diagnosis Name cannot exceed 200 characters.")]
        public string DiagnosisName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
