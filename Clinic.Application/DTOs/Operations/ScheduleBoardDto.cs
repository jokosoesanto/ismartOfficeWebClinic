using System;

namespace Clinic.Application.DTOs.Operations
{
    public class ScheduleBoardDto
    {
        public Guid ScheduleId { get; set; }
        
        // Doctor Details
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string? Specialty { get; set; }
        public string? DoctorColor { get; set; }
        
        // Location & Chair
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public string? Chair { get; set; } // Will be filled if chair mapping exists, otherwise empty/N/A
        
        // Time & Date
        public int DayOfWeek { get; set; }
        public string DayName { get; set; } = null!;
        public DateTime? SpecificDate { get; set; } // Can be null if viewing generic schedule, or filled if filtered by Date
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? BreakStart { get; set; }
        public TimeSpan? BreakEnd { get; set; }
        
        // Status & Operational
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; } // From Schedule
        public string Status { get; set; } = "Available"; // Derived status (Available, Break, Leave, Inactive, InSession)
    }
}
