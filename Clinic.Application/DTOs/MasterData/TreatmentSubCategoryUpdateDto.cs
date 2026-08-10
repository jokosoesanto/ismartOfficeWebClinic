using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class TreatmentSubCategoryUpdateDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        public string SubCategoryCode { get; set; } = string.Empty; // Read-only

        [Required(ErrorMessage = "SubCategory Name is required.")]
        [StringLength(100, ErrorMessage = "SubCategory Name cannot exceed 100 characters.")]
        [Display(Name = "SubCategory Name")]
        public string SubCategoryName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Display Order is required.")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
