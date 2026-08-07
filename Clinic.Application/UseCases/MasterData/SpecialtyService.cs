using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.UseCases.MasterData
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ISpecialtyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SpecialtyService(ISpecialtyRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Specialty>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Specialty>> GetAllActiveAsync()
        {
            return await _repository.GetAllActiveAsync();
        }

        public async Task<Specialty?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Specialty specialty)
        {
            var existing = await _repository.GetByCodeAsync(specialty.Code);
            if (existing != null) throw new Exception("Specialty with this Code already exists.");

            await _repository.AddAsync(specialty);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Specialty specialty)
        {
            _repository.Update(specialty);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, Guid deletedBy)
        {
            var specialty = await _repository.GetByIdAsync(id);
            if (specialty != null && !specialty.IsSystem)
            {
                specialty.IsDeleted = true;
                specialty.DeletedAt = DateTime.UtcNow;
                specialty.DeletedBy = deletedBy;
                _repository.Update(specialty);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
