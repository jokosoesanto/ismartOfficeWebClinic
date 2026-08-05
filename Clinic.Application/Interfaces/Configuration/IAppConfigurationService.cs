using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Configuration;

namespace Clinic.Application.Interfaces.Configuration
{
    public interface IAppConfigurationService
    {
        Task<IEnumerable<AppConfigurationDto>> GetAllAsync();
        Task<IEnumerable<AppConfigurationDto>> GetByCategoryAsync(string category);
        Task<AppConfigurationDto?> GetByKeyAsync(string key);
        Task<string> GetValueAsync(string key, string defaultValue = "");
        Task<int> GetIntValueAsync(string key, int defaultValue = 0);
        Task UpdateAsync(AppConfigurationDto dto, Guid? updatedBy);
    }
}

