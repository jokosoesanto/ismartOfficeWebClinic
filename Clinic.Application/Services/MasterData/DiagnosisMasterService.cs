using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.DTOs.MasterData;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Services.MasterData
{
    public class DiagnosisMasterService : IDiagnosisMasterService
    {
        private readonly IDiagnosisMasterRepository _repository;

        public DiagnosisMasterService(IDiagnosisMasterRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DiagnosisMasterDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new DiagnosisMasterDto
            {
                Id = e.Id,
                DiagnosisCode = e.DiagnosisCode,
                DiagnosisName = e.DiagnosisName,
                Description = e.Description,
                IsActive = e.IsActive
            });
        }

        public async Task<DiagnosisMasterDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new DiagnosisMasterDto
            {
                Id = entity.Id,
                DiagnosisCode = entity.DiagnosisCode,
                DiagnosisName = entity.DiagnosisName,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }

        public async Task<DiagnosisMasterDto> CreateAsync(DiagnosisMasterCreateDto dto, Guid? createdBy = null)
        {
            if (await _repository.CodeExistsAsync(dto.DiagnosisCode))
            {
                throw new InvalidOperationException($"Diagnosis Code '{dto.DiagnosisCode}' already exists.");
            }

            var entity = new DiagnosisMaster
            {
                DiagnosisCode = dto.DiagnosisCode,
                DiagnosisName = dto.DiagnosisName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            await _repository.AddAsync(entity);

            return new DiagnosisMasterDto
            {
                Id = entity.Id,
                DiagnosisCode = entity.DiagnosisCode,
                DiagnosisName = entity.DiagnosisName,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }

        public async Task UpdateAsync(Guid id, DiagnosisMasterCreateDto dto, Guid? updatedBy = null)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Diagnosis Master with ID {id} not found.");

            if (await _repository.CodeExistsAsync(dto.DiagnosisCode, id))
            {
                throw new InvalidOperationException($"Diagnosis Code '{dto.DiagnosisCode}' already exists.");
            }

            entity.DiagnosisCode = dto.DiagnosisCode;
            entity.DiagnosisName = dto.DiagnosisName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = updatedBy;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id, Guid? deletedBy = null)
        {
            await _repository.DeleteAsync(id, deletedBy);
        }

        public async Task<bool> ToggleActiveStatusAsync(Guid id, Guid? updatedBy = null)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Diagnosis Master with ID {id} not found.");

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
    }
}
