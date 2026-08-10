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
    public class TreatmentSubCategoryService : ITreatmentSubCategoryService
    {
        private readonly ITreatmentSubCategoryRepository _repository;
        private readonly ITreatmentCategoryRepository _categoryRepository;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly IUnitOfWork _unitOfWork;

        public TreatmentSubCategoryService(
            ITreatmentSubCategoryRepository repository,
            ITreatmentCategoryRepository categoryRepository,
            INumberSequenceService numberSequenceService,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _numberSequenceService = numberSequenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TreatmentSubCategoryDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public async Task<IEnumerable<TreatmentSubCategoryDto>> GetByCategoryIdAsync(Guid categoryId)
        {
            var entities = await _repository.GetByCategoryIdAsync(categoryId);
            return entities.Select(MapToDto);
        }

        public async Task<TreatmentSubCategoryDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return null;
            return MapToDto(entity);
        }

        public async Task<TreatmentSubCategory> CreateAsync(TreatmentSubCategoryCreateDto dto, Guid userId)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null || category.IsDeleted || !category.IsActive)
                throw new InvalidOperationException("Parent Category is invalid or inactive.");

            if (!await _repository.IsNameUniqueAsync(dto.CategoryId, dto.SubCategoryName))
                throw new InvalidOperationException("SubCategory name must be unique within the selected Category.");

            var codeResult = await _numberSequenceService.GenerateSequenceAsync("TSC");

            var entity = new TreatmentSubCategory
            {
                Id = Guid.NewGuid(),
                CategoryId = dto.CategoryId,
                SubCategoryCode = codeResult,
                SubCategoryName = dto.SubCategoryName,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                IsDeleted = false
            };

            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity;
        }

        public async Task<TreatmentSubCategory> UpdateAsync(TreatmentSubCategoryUpdateDto dto, Guid userId)
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null || existing.IsDeleted)
                throw new InvalidOperationException("SubCategory not found.");

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null || category.IsDeleted)
                throw new InvalidOperationException("Parent Category is invalid.");

            if (!await _repository.IsNameUniqueAsync(dto.CategoryId, dto.SubCategoryName, dto.Id))
                throw new InvalidOperationException("SubCategory name must be unique within the selected Category.");

            existing.CategoryId = dto.CategoryId;
            existing.SubCategoryName = dto.SubCategoryName;
            existing.Description = dto.Description;
            existing.DisplayOrder = dto.DisplayOrder;
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
                throw new InvalidOperationException("SubCategory not found.");

            existing.IsDeleted = true;
            existing.IsActive = false;
            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = userId;

            await _repository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        private static TreatmentSubCategoryDto MapToDto(TreatmentSubCategory entity)
        {
            return new TreatmentSubCategoryDto
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.CategoryName ?? string.Empty,
                SubCategoryCode = entity.SubCategoryCode,
                SubCategoryName = entity.SubCategoryName,
                Description = entity.Description,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive
            };
        }
    }
}
