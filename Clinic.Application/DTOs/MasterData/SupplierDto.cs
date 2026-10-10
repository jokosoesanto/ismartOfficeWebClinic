using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Supplier Code is required.")]
        [StringLength(50, ErrorMessage = "Supplier Code cannot exceed 50 characters.")]
        public string Code { get; set; } = null!;
        
        [Required(ErrorMessage = "Supplier Name is required.")]
        [StringLength(200, ErrorMessage = "Supplier Name cannot exceed 200 characters.")]
        public string Name { get; set; } = null!;
        
        [StringLength(100, ErrorMessage = "Contact Person cannot exceed 100 characters.")]
        public string? ContactPerson { get; set; }
        
        [StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters.")]
        public string? Phone { get; set; }
        
        [EmailAddress(ErrorMessage = "Invalid Email Address format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? Email { get; set; }
        
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
