using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.System
{
    public class MasterReference
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string Category { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        
        public int SortOrder { get; set; } = 0;
        
        // Support for hierarchical references
        public Guid? ParentId { get; set; }
        public MasterReference? Parent { get; set; }
        public ICollection<MasterReference> Children { get; set; } = new List<MasterReference>();

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Standard Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
