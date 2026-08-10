using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class TreatmentCatalogCreateDto
    {
        [Required(ErrorMessage = "Category is required.")]
        public Guid CategoryId { get; set; }
        
        [Required(ErrorMessage = "SubCategory is required.")]
        public Guid SubCategoryId { get; set; }
        
        [Required(ErrorMessage = "Service Type is required.")]
        public Guid ServiceTypeId { get; set; }

        [Required(ErrorMessage = "Treatment Name is required.")]
        [StringLength(150, ErrorMessage = "Treatment Name cannot exceed 150 characters.")]
        public string TreatmentName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Default Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Default Price must be a positive value or zero.")]
        public decimal DefaultPrice { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than zero.")]
        public int DurationInMinutes { get; set; }

        public bool RequiresTooth { get; set; }
        
        public bool RequiresSurface { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
