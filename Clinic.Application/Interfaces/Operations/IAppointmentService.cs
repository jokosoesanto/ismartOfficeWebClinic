using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAsync(AppointmentDto dto, Guid userId);
        Task<IEnumerable<AppointmentDto>> GetAllAsync();
    }
}
