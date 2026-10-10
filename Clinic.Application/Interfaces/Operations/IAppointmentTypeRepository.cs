using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IAppointmentTypeRepository
    {
        Task<IEnumerable<AppointmentType>> GetAllAsync();
        Task<AppointmentType?> GetByIdAsync(Guid id);
        Task<AppointmentType?> GetByCodeAsync(string code);
        Task AddAsync(AppointmentType entity);
        Task UpdateAsync(AppointmentType entity);
        Task<bool> ExistsAsync(Guid id);
    }
}
