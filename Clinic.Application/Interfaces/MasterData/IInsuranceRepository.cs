using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IInsuranceRepository
    {
        Task<Insurance?> GetByIdAsync(Guid id);
        Task<List<Insurance>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
        Task AddAsync(Insurance insurance);
        void Update(Insurance insurance);
        void Delete(Insurance insurance);
    }
}
