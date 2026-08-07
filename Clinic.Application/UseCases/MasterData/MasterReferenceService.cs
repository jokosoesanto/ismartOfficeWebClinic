using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.UseCases.MasterData
{
    public class MasterReferenceService : IMasterReferenceService
    {
        private readonly IMasterReferenceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MasterReferenceService> _logger;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public MasterReferenceService(IMasterReferenceRepository repository, IUnitOfWork unitOfWork, IMemoryCache cache, ILogger<MasterReferenceService> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<MasterReference>> GetByCategoryAsync(string category, bool activeOnly = true, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"MasterReference_{category}_{(activeOnly ? "Active" : "All")}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<MasterReference>? cachedResult) || cachedResult == null)
            {
                cachedResult = await _repository.GetByCategoryAsync(category, activeOnly, cancellationToken);
                _cache.Set(cacheKey, cachedResult, CacheDuration);
            }

            return cachedResult;
        }

        public async Task<MasterReference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<MasterReference?> GetByCodeAsync(string category, string code, CancellationToken cancellationToken = default)
        {
            return await _repository.GetByCodeAsync(category, code, cancellationToken);
        }

        public async Task<MasterReference> CreateAsync(MasterReference masterReference, Guid userId, CancellationToken cancellationToken = default)
        {
            var exists = await _repository.AnyAsync(masterReference.Category, masterReference.Code, null, cancellationToken);
            if (exists)
                throw new InvalidOperationException($"Master Reference with code '{masterReference.Code}' already exists in category '{masterReference.Category}'.");

            masterReference.Id = Guid.NewGuid();
            masterReference.CreatedAt = DateTime.UtcNow;
            masterReference.CreatedBy = userId;
            masterReference.IsSystem = false; 

            await _repository.AddAsync(masterReference, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            ClearCache(masterReference.Category);

            return masterReference;
        }

        public async Task<MasterReference> UpdateAsync(MasterReference masterReference, Guid userId, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(masterReference.Id, cancellationToken);
            if (existing == null)
                throw new InvalidOperationException("Master reference not found.");

            if (existing.IsSystem)
            {
                if (!masterReference.IsActive)
                    throw new InvalidOperationException("System records cannot be deactivated.");
                    
                // System records can only update SortOrder and Description
                existing.SortOrder = masterReference.SortOrder;
                existing.Description = masterReference.Description;
            }
            else
            {
                var codeConflict = await _repository.AnyAsync(masterReference.Category, masterReference.Code, masterReference.Id, cancellationToken);
                if (codeConflict)
                    throw new InvalidOperationException($"Master Reference with code '{masterReference.Code}' already exists in category '{masterReference.Category}'.");

                existing.Category = masterReference.Category;
                existing.Code = masterReference.Code;
                existing.Name = masterReference.Name;
                existing.Description = masterReference.Description;
                existing.SortOrder = masterReference.SortOrder;
                existing.IsActive = masterReference.IsActive;
                existing.ParentId = masterReference.ParentId;
                existing.EffectiveFrom = masterReference.EffectiveFrom;
                existing.EffectiveTo = masterReference.EffectiveTo;
            }

            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = userId;

            await _repository.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            ClearCache(existing.Category);
            
            if (!existing.IsSystem && existing.Category != masterReference.Category)
            {
                ClearCache(masterReference.Category);
            }

            return existing;
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                throw new InvalidOperationException("Master reference not found.");

            if (existing.IsSystem)
                throw new InvalidOperationException("System records cannot be deleted.");

            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = userId;

            await _repository.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            ClearCache(existing.Category);
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetCategoriesAsync(cancellationToken);
        }

        private void ClearCache(string category)
        {
            _cache.Remove($"MasterReference_{category}_Active");
            _cache.Remove($"MasterReference_{category}_All");
        }
    }
}
