using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IAppointmentTypeService
    {
        Task<IEnumerable<AppointmentTypeDto>> GetAllAsync();
        Task<AppointmentTypeDto?> GetByIdAsync(Guid id);
        Task<AppointmentTypeDto> CreateAsync(AppointmentTypeCreateDto dto, Guid? createdBy = null);
        Task UpdateAsync(Guid id, AppointmentTypeCreateDto dto, Guid? updatedBy = null);
        Task DeleteAsync(Guid id, Guid? deletedBy = null);
        Task<bool> ToggleActiveStatusAsync(Guid id, Guid? updatedBy = null);
        Task<bool> ExistsAsync(Guid id);
    }
}
