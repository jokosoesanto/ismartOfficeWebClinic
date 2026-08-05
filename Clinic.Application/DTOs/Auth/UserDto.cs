using System;
using System.Collections.Generic;

namespace Clinic.Application.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Notes { get; set; }
        public bool MustChangePassword { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public Guid? PrimaryLocationId { get; set; }
        public string PrimaryLocationName { get; set; } = string.Empty;
        public List<Guid> AccessibleLocationIds { get; set; } = new();
        
        public List<Guid> RoleIds { get; set; } = new();
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }
}
