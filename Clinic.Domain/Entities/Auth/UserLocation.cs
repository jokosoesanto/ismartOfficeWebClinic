using System;

namespace Clinic.Domain.Entities.Auth
{
    public class UserLocation
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid LocationId { get; set; }
        public Clinic.Domain.Entities.MasterData.Location Location { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
