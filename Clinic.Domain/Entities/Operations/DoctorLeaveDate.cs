using System;

namespace Clinic.Domain.Entities.Operations
{
    public class DoctorLeaveDate
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DoctorLeaveRequestId { get; set; }
        public DoctorLeaveRequest? DoctorLeaveRequest { get; set; }

        public DateTime Date { get; set; }
    }
}
