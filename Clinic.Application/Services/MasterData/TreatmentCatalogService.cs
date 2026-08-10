using Clinic.Application.DTOs.MasterData;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Application.Services.MasterData
{
    public class TreatmentCatalogService : ITreatmentCatalogService
    {
        private readonly ITreatmentCatalogRepository _repository;
        private readonly ITreatmentCategoryRepository _categoryRepository;
        private readonly ITreatmentSubCategoryRepository _subCategoryRepository;
        private readonly IMasterReferenceRepository _masterReferenceRepository;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly IUnitOfWork _unitOfWork;

        public TreatmentCatalogService(
            ITreatmentCatalogRepository repository,
            ITreatmentCategoryRepository categoryRepository,
            ITreatmentSubCategoryRepository subCategoryRepository,
            IMasterReferenceRepository masterReferenceRepository,
            INumberSequenceService numberSequenceService,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _masterReferenceRepository = masterReferenceRepository;
            _numberSequenceService = numberSequenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TreatmentCatalogDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public async Task<IEnumerable<TreatmentCatalogDto>> GetBySubCategoryIdAsync(Guid subCategoryId)
        {
            var entities = await _repository.GetBySubCategoryIdAsync(subCategoryId);
            return entities.Select(MapToDto);
        }

        public async Task<TreatmentCatalogDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return null;
            return MapToDto(entity);
        }

        public async Task<TreatmentCatalog> CreateAsync(TreatmentCatalogCreateDto dto, Guid userId)
        {
            await ValidateRelationsAsync(dto.CategoryId, dto.SubCategoryId, dto.ServiceTypeId);

            if (!await _repository.IsNameUniqueAsync(dto.SubCategoryId, dto.TreatmentName))
                throw new InvalidOperationException("Treatment name must be unique within the selected SubCategory.");

            if (dto.RequiresSurface && !dto.RequiresTooth)
            {
                // Business rule: If RequiresSurface = true, RequiresTooth must be true
                dto.RequiresTooth = true;
            }

            var codeResult = await _numberSequenceService.GenerateSequenceAsync("TRT");

            var entity = new TreatmentCatalog
            {
                Id = Guid.NewGuid(),
                TreatmentCode = codeResult,
                CategoryId = dto.CategoryId,
                SubCategoryId = dto.SubCategoryId,
                ServiceTypeId = dto.ServiceTypeId,
                TreatmentName = dto.TreatmentName,
                Description = dto.Description,
                DefaultPrice = dto.DefaultPrice,
                DurationInMinutes = dto.DurationInMinutes,
                RequiresTooth = dto.RequiresTooth,
                RequiresSurface = dto.RequiresSurface,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                IsDeleted = false
            };

            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity;
        }

        public async Task<TreatmentCatalog> UpdateAsync(TreatmentCatalogUpdateDto dto, Guid userId)
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null || existing.IsDeleted)
                throw new InvalidOperationException("Treatment Catalog not found.");

            await ValidateRelationsAsync(dto.CategoryId, dto.SubCategoryId, dto.ServiceTypeId);

            if (!await _repository.IsNameUniqueAsync(dto.SubCategoryId, dto.TreatmentName, dto.Id))
                throw new InvalidOperationException("Treatment name must be unique within the selected SubCategory.");

            if (dto.RequiresSurface && !dto.RequiresTooth)
            {
                dto.RequiresTooth = true;
            }

            existing.CategoryId = dto.CategoryId;
            existing.SubCategoryId = dto.SubCategoryId;
            existing.ServiceTypeId = dto.ServiceTypeId;
            existing.TreatmentName = dto.TreatmentName;
            existing.Description = dto.Description;
            existing.DefaultPrice = dto.DefaultPrice;
            existing.DurationInMinutes = dto.DurationInMinutes;
            existing.RequiresTooth = dto.RequiresTooth;
            existing.RequiresSurface = dto.RequiresSurface;
            existing.IsActive = dto.IsActive;
            
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = userId;

            await _repository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return existing;
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null || existing.IsDeleted)
                throw new InvalidOperationException("Treatment Catalog not found.");

            existing.IsDeleted = true;
            existing.IsActive = false;
            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = userId;

            await _repository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task ValidateRelationsAsync(Guid categoryId, Guid subCategoryId, Guid serviceTypeId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null || category.IsDeleted || !category.IsActive)
                throw new InvalidOperationException("Parent Category is invalid or inactive.");

            var subCategory = await _subCategoryRepository.GetByIdAsync(subCategoryId);
            if (subCategory == null || subCategory.IsDeleted)
                throw new InvalidOperationException("SubCategory is invalid.");

            if (subCategory.CategoryId != categoryId)
                throw new InvalidOperationException("The selected SubCategory does not belong to the selected Category.");

            var serviceType = await _masterReferenceRepository.GetByIdAsync(serviceTypeId);
            if (serviceType == null || serviceType.IsDeleted || !serviceType.IsActive || serviceType.Category != "ServiceType")
                throw new InvalidOperationException("Service Type is invalid or inactive.");
        }

        private static TreatmentCatalogDto MapToDto(TreatmentCatalog entity)
        {
            return new TreatmentCatalogDto
            {
                Id = entity.Id,
                TreatmentCode = entity.TreatmentCode,
                TreatmentName = entity.TreatmentName,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.CategoryName ?? string.Empty,
                SubCategoryId = entity.SubCategoryId,
                SubCategoryName = entity.SubCategory?.SubCategoryName ?? string.Empty,
                ServiceTypeId = entity.ServiceTypeId,
                ServiceTypeName = entity.ServiceType?.Name ?? string.Empty,
                DefaultPrice = entity.DefaultPrice,
                DurationInMinutes = entity.DurationInMinutes,
                RequiresTooth = entity.RequiresTooth,
                RequiresSurface = entity.RequiresSurface,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }
    }
}
