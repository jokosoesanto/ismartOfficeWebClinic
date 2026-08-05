using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Configuration;

namespace Clinic.Application.Interfaces.Configuration
{
    public interface IAppConfigurationRepository
    {
        Task<IEnumerable<AppConfiguration>> GetAllAsync();
        Task<IEnumerable<AppConfiguration>> GetByCategoryAsync(string category);
        Task<AppConfiguration?> GetByKeyAsync(string key);
        Task AddAsync(AppConfiguration config);
        Task UpdateAsync(AppConfiguration config);
    }
}

