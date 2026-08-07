using System;

namespace Clinic.Domain.Entities.MasterData
{
    public class DoctorLocation
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public Guid LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}