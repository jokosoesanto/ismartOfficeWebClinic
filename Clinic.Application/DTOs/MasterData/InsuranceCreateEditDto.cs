using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class InsuranceCreateEditDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Insurance Name is required.")]
        [StringLength(150, ErrorMessage = "Insurance Name cannot exceed 150 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Insurance Group is required.")]
        public Guid GroupId { get; set; }

        [Required(ErrorMessage = "Primary Coverage is required.")]
        [StringLength(250, ErrorMessage = "Primary Coverage cannot exceed 250 characters.")]
        public string PrimaryCoverage { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Office Address cannot exceed 500 characters.")]
        public string? OfficeAddress { get; set; }

        [StringLength(100, ErrorMessage = "Contact Name cannot exceed 100 characters.")]
        public string? ContactName { get; set; }

        [StringLength(50, ErrorMessage = "Contact Number cannot exceed 50 characters.")]
        public string? ContactNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address format.")]
        [StringLength(150, ErrorMessage = "Contact Email cannot exceed 150 characters.")]
        public string? ContactEmail { get; set; }

        [StringLength(1000, ErrorMessage = "Remark cannot exceed 1000 characters.")]
        public string? Remark { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(100, ErrorMessage = "External System cannot exceed 100 characters.")]
        public string? ExternalSystem { get; set; }

        [StringLength(100, ErrorMessage = "External Identifier cannot exceed 100 characters.")]
        public string? ExternalIdentifier { get; set; }
    }
}
