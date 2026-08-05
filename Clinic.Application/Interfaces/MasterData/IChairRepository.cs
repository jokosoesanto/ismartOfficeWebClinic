using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IChairRepository
    {
        Task<Chair?> GetByIdAsync(Guid id);
        Task<Chair?> GetByCodeAsync(string code);
        Task<IEnumerable<Chair>> GetAllAsync();
        Task AddAsync(Chair chair);
        Task UpdateAsync(Chair chair);
        // Note: For appointment validation later
        Task<bool> HasAppointmentsAsync(Guid id);
    }
}
