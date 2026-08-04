using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.Auth
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
