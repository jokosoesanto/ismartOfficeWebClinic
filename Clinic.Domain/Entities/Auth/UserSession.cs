using System;

namespace Clinic.Domain.Entities.Auth
{
    public class UserSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public string SessionToken { get; set; } = null!;
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        
        public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
    }
}
