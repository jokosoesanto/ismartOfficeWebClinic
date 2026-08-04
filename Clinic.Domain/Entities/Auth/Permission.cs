using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.Auth
{
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Code, e.g. Patient.View, Patient.Create
        /// </summary>
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!; // Kept for backward compatibility if needed, or renamed to DisplayName.
        public string DisplayName { get; set; } = null!;
        public string? Description { get; set; }
        public string Category { get; set; } = "General";
        public string Module { get; set; } = "General";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public PermissionType Type { get; set; } = PermissionType.System;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
