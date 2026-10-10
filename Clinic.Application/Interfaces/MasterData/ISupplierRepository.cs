using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync(bool includeDeleted = false);
        Task<Supplier?> GetByIdAsync(Guid id);
        Task<Supplier?> GetByCodeAsync(string code);
        Task<Supplier> AddAsync(Supplier entity);
        Task UpdateAsync(Supplier entity);
    }
}
