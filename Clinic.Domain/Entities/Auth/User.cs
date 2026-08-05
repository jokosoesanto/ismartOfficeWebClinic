using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.Auth
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = null!;
        public string NormalizedUsername { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Salt { get; set; } // May not be needed with Identity PasswordHasher, but kept for legacy/custom compat
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string NormalizedEmail { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? DisplayName { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsLocked { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutUntil { get; set; }
        public bool MustChangePassword { get; set; }
        public DateTime? LastPasswordChangedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string PermissionVersion { get; set; } = Guid.NewGuid().ToString("N");

        public Guid? PrimaryLocationId { get; set; }
        public Clinic.Domain.Entities.MasterData.Location? PrimaryLocation { get; set; }

        public ICollection<UserLocation> UserAccessibleLocations { get; set; } = new List<UserLocation>();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    }
}
