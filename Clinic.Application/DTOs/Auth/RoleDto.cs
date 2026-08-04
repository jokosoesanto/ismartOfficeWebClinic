using System;
using System.Collections.Generic;

namespace Clinic.Application.DTOs.Auth
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UsersCount { get; set; }
        public int PermissionsCount { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }
}
