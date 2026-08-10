using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Domain.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Clinic.Application.UseCases.MasterData
{
    public class TreatmentCategoryService : ITreatmentCategoryService
    {
        private readonly ITreatmentCategoryRepository _repository;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly IUnitOfWork _unitOfWork;

        public TreatmentCategoryService(
            ITreatmentCategoryRepository repository,
            INumberSequenceService numberSequenceService,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _numberSequenceService = numberSequenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TreatmentCategory>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities;
        }

        public async Task<TreatmentCategory?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return null;
            return entity;
        }

        public async Task<TreatmentCategory> CreateAsync(TreatmentCategory category, Guid userId)
        {
            if (!await _repository.IsNameUniqueAsync(category.CategoryName))
                throw new InvalidOperationException("Category name must be unique.");

            var codeResult = await _numberSequenceService.GenerateSequenceAsync("TC");
            category.CategoryCode = codeResult;
            
            category.CreatedAt = DateTime.UtcNow;
            category.CreatedBy = userId;
            category.IsDeleted = false;

            await _repository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category;
        }

        public async Task<TreatmentCategory> UpdateAsync(Clinic.Application.DTOs.MasterData.TreatmentCategoryUpdateDto category, Guid userId)
        {
            var existing = await _repository.GetByIdAsync(category.Id);
            if (existing == null || existing.IsDeleted)
                throw new InvalidOperationException("Category not found.");

            if (!await _repository.IsNameUniqueAsync(category.CategoryName, category.Id))
                throw new InvalidOperationException("Category name must be unique.");

            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            existing.DisplayOrder = category.DisplayOrder;
            existing.IsActive = category.IsActive;
            
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
                throw new InvalidOperationException("Category not found.");

            existing.IsDeleted = true;
            existing.IsActive = false;
            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = userId;

            await _repository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null)
        {
            return await _repository.IsNameUniqueAsync(name, excludeId);
        }
    }
}
