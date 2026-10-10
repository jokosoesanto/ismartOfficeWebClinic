using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Domain.Entities.MasterData
{
    public class Supplier
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = null!;
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
        
        [MaxLength(100)]
        public string? ContactPerson { get; set; }
        
        [MaxLength(50)]
        public string? Phone { get; set; }
        
        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        
        [MaxLength(500)]
        public string? Address { get; set; }
        
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
