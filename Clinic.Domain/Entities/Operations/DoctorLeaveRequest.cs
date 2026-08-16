using System;
using System.Collections.Generic;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Domain.Entities.Operations
{
    public class DoctorLeaveRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public string? Reason { get; set; }

        // 1:N child dates
        public ICollection<DoctorLeaveDate> LeaveDates { get; set; } = new List<DoctorLeaveDate>();

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
