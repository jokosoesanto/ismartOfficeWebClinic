using System;
using Clinic.Domain.Entities.MasterData;
using Clinic.Domain.Enums;

namespace Clinic.Domain.Entities.Operations
{
    public class Appointment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign Keys
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }

        public Guid DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public Guid LocationId { get; set; }
        public Location? Location { get; set; }

        public Guid ChairId { get; set; }
        public Chair? Chair { get; set; }

        // Date and Time
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Status (persisted as string in EF Core)
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Schedule;

        public string? Notes { get; set; }

        // Audit fields
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
