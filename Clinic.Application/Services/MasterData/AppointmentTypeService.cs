using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.DTOs.MasterData;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Services.MasterData
{
    public class AppointmentTypeService : IAppointmentTypeService
    {
        private readonly IAppointmentTypeRepository _repository;

        public AppointmentTypeService(IAppointmentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AppointmentTypeDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public async Task<AppointmentTypeDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<AppointmentTypeDto> CreateAsync(AppointmentTypeCreateDto dto, Guid? createdBy = null)
        {
            var existing = await _repository.GetByCodeAsync(dto.Code);
            if (existing != null)
                throw new InvalidOperationException($"Appointment Type with code '{dto.Code}' already exists.");

            var entity = new AppointmentType
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                Name = dto.Name,
                DisplayName = dto.DisplayName,
                Description = dto.Description,
                Complexity = dto.Complexity,
                DurationInMinutes = dto.DurationInMinutes,
                IsActive = dto.IsActive,
                IsSystem = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            await _repository.AddAsync(entity);
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, AppointmentTypeCreateDto dto, Guid? updatedBy = null)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"Appointment Type {id} not found.");

            if (entity.Code != dto.Code)
            {
                var existing = await _repository.GetByCodeAsync(dto.Code);
                if (existing != null)
                    throw new InvalidOperationException($"Appointment Type with code '{dto.Code}' already exists.");
            }

            entity.Code = dto.Code;
            entity.Name = dto.Name;
            entity.DisplayName = dto.DisplayName;
            entity.Description = dto.Description;
            entity.Complexity = dto.Complexity;
            entity.DurationInMinutes = dto.DurationInMinutes;
            entity.IsActive = dto.IsActive;
            
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = updatedBy;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id, Guid? deletedBy = null)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"Appointment Type {id} not found.");
            
            if (entity.IsSystem)
                throw new InvalidOperationException("System Appointment Types cannot be deleted.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = deletedBy;
            entity.IsActive = false;

            await _repository.UpdateAsync(entity);
        }

        public async Task<bool> ToggleActiveStatusAsync(Guid id, Guid? updatedBy = null)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"Appointment Type {id} not found.");
            
            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = updatedBy;

            await _repository.UpdateAsync(entity);
            return entity.IsActive;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        private static AppointmentTypeDto MapToDto(AppointmentType entity)
        {
            return new AppointmentTypeDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                Complexity = entity.Complexity,
                DurationInMinutes = entity.DurationInMinutes,
                IsActive = entity.IsActive,
                IsSystem = entity.IsSystem
            };
        }
    }
}
