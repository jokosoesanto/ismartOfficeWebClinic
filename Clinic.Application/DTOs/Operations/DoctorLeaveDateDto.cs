using System;

namespace Clinic.Application.DTOs.Operations
{
    public class DoctorLeaveDateDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledAt { get; set; }
        public Guid? CancelledBy { get; set; }
        public string? CancellationReason { get; set; }
    }
}
