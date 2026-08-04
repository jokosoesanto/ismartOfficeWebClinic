using System;
using System.Collections.Generic;

namespace Clinic.Application.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool MustChangePassword { get; set; }
        
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }
}
