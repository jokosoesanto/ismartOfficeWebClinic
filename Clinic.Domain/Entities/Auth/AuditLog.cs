using System;

namespace Clinic.Domain.Entities.Auth
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public string Action { get; set; } = null!;
        public string Module { get; set; } = null!;
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Browser { get; set; }
        public string? Device { get; set; }
        public string? UserAgent { get; set; }
        public string? OperatingSystem { get; set; }
        public string? IPAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
