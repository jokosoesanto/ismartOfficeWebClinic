using System;

namespace Clinic.Application.DTOs.Operations
{
    public class PatientTimelineItemDto
    {
        public DateTime EventDate { get; set; }
        public string EventType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public Guid ReferenceId { get; set; }
        public string? Icon { get; set; }
    }
}
