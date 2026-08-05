using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Configuration;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Configuration;

namespace Clinic.Application.UseCases.Configuration
{
    public class AppConfigurationService : IAppConfigurationService
    {
        private readonly IAppConfigurationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AppConfigurationService(IAppConfigurationRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AppConfigurationDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new AppConfigurationDto
            {
                Id = e.Id,
                Category = e.Category,
                Key = e.Key,
                Value = e.Value,
                Description = e.Description
            });
        }

        public async Task<IEnumerable<AppConfigurationDto>> GetByCategoryAsync(string category)
        {
            var entities = await _repository.GetByCategoryAsync(category);
            return entities.Select(e => new AppConfigurationDto
            {
                Id = e.Id,
                Category = e.Category,
                Key = e.Key,
                Value = e.Value,
                Description = e.Description
            });
        }

        public async Task<AppConfigurationDto?> GetByKeyAsync(string key)
        {
            var entity = await _repository.GetByKeyAsync(key);
            if (entity == null) return null;

            return new AppConfigurationDto
            {
                Id = entity.Id,
                Category = entity.Category,
                Key = entity.Key,
                Value = entity.Value,
                Description = entity.Description
            };
        }

        public async Task<string> GetValueAsync(string key, string defaultValue = "")
        {
            var entity = await _repository.GetByKeyAsync(key);
            return entity != null ? entity.Value : defaultValue;
        }

        public async Task<int> GetIntValueAsync(string key, int defaultValue = 0)
        {
            var value = await GetValueAsync(key);
            if (int.TryParse(value, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        public async Task UpdateAsync(AppConfigurationDto dto, Guid? updatedBy)
        {
            var entity = await _repository.GetByKeyAsync(dto.Key);
            if (entity != null)
            {
                entity.Value = dto.Value;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = updatedBy;
                await _repository.UpdateAsync(entity);
            }
            else
            {
                // Create if not exists
                var newEntity = new Clinic.Domain.Entities.Configuration.AppConfiguration
                {
                    Id = Guid.NewGuid(),
                    Category = dto.Category,
                    Key = dto.Key,
                    Value = dto.Value,
                    Description = dto.Description,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = updatedBy
                };
                await _repository.AddAsync(newEntity);
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

