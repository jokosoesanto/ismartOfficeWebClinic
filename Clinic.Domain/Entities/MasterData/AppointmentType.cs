using System;
using System.Collections.Generic;
using Clinic.Domain.Enums;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Domain.Entities.MasterData
{
    public class AppointmentType
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        
        public AppointmentComplexity Complexity { get; set; } = AppointmentComplexity.Medium;

        public int DurationInMinutes { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsSystem { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
