using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IDoctorLeaveRequestRepository
    {
        Task<DoctorLeaveRequest?> GetByIdAsync(Guid id);
        Task<IEnumerable<DoctorLeaveRequest>> GetAllAsync();
        void Add(DoctorLeaveRequest request);
        void Update(DoctorLeaveRequest request);
        Task<DoctorLeaveRequest?> GetByLeaveDateIdAsync(Guid leaveDateId);

        /// <summary>
        /// Returns true if the given doctor already has an active (non-deleted) leave date
        /// on any of the specified dates. Optionally exclude a specific request for edit scenarios.
        /// </summary>
        Task<List<DateTime>> GetDuplicateDatesAsync(Guid doctorId, IEnumerable<DateTime> dates, Guid? excludeRequestId = null);

        /// <summary>
        /// Returns the dates (from the provided list) on which the doctor has existing appointments.
        /// </summary>
        Task<List<DateTime>> GetDatesWithAppointmentsAsync(Guid doctorId, IEnumerable<DateTime> dates);
    }
}
