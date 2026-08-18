using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IDoctorLeaveRequestService
    {
        Task<DoctorLeaveRequestDto> CreateAsync(DoctorLeaveRequestDto dto, Guid userId);
        Task<DoctorLeaveRequestDto> UpdateAsync(DoctorLeaveRequestDto dto, Guid userId);
        Task<IEnumerable<DoctorLeaveRequestDto>> GetAllAsync();
        Task<DoctorLeaveRequestDto?> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id, Guid deletedBy);
        Task CancelLeaveDateAsync(Guid leaveDateId, string? reason, Guid cancelledBy);
    }
}
