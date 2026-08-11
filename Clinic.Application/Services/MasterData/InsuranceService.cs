using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.DTOs.MasterData;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Services.MasterData
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IInsuranceRepository _insuranceRepository;
        private readonly Clinic.Application.Interfaces.IUnitOfWork _unitOfWork;

        public InsuranceService(IInsuranceRepository insuranceRepository, Clinic.Application.Interfaces.IUnitOfWork unitOfWork)
        {
            _insuranceRepository = insuranceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<InsuranceListDto>> GetAllAsync()
        {
            var insurances = await _insuranceRepository.GetAllAsync();
            return insurances.Select(i => new InsuranceListDto
            {
                Id = i.Id,
                Name = i.Name,
                GroupName = i.Group?.Name ?? string.Empty,
                PrimaryCoverage = i.PrimaryCoverage,
                ContactName = i.ContactName,
                ContactNumber = i.ContactNumber,
                IsActive = i.IsActive,
                ExternalIdentifier = i.ExternalIdentifier
            }).ToList();
        }

        public async Task<InsuranceDto?> GetByIdAsync(Guid id)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(id);
            if (insurance == null) return null;

            return new InsuranceDto
            {
                Id = insurance.Id,
                Name = insurance.Name,
                GroupId = insurance.GroupId,
                GroupName = insurance.Group?.Name,
                PrimaryCoverage = insurance.PrimaryCoverage,
                OfficeAddress = insurance.OfficeAddress,
                ContactName = insurance.ContactName,
                ContactNumber = insurance.ContactNumber,
                ContactEmail = insurance.ContactEmail,
                Remark = insurance.Remark,
                IsActive = insurance.IsActive,
                ExternalSystem = insurance.ExternalSystem,
                ExternalIdentifier = insurance.ExternalIdentifier
            };
        }

        public async Task<InsuranceCreateEditDto?> GetForEditAsync(Guid id)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(id);
            if (insurance == null) return null;

            return new InsuranceCreateEditDto
            {
                Id = insurance.Id,
                Name = insurance.Name,
                GroupId = insurance.GroupId,
                PrimaryCoverage = insurance.PrimaryCoverage,
                OfficeAddress = insurance.OfficeAddress,
                ContactName = insurance.ContactName,
                ContactNumber = insurance.ContactNumber,
                ContactEmail = insurance.ContactEmail,
                Remark = insurance.Remark,
                IsActive = insurance.IsActive,
                ExternalSystem = insurance.ExternalSystem,
                ExternalIdentifier = insurance.ExternalIdentifier
            };
        }

        public async Task<Guid> CreateAsync(InsuranceCreateEditDto dto)
        {
            if (await _insuranceRepository.ExistsByNameAsync(dto.Name))
            {
                throw new ApplicationException($"Insurance with name '{dto.Name}' already exists.");
            }

            var insurance = new Insurance
            {
                Name = dto.Name,
                GroupId = dto.GroupId,
                PrimaryCoverage = dto.PrimaryCoverage,
                OfficeAddress = dto.OfficeAddress,
                ContactName = dto.ContactName,
                ContactNumber = dto.ContactNumber,
                ContactEmail = dto.ContactEmail,
                Remark = dto.Remark,
                IsActive = dto.IsActive,
                ExternalSystem = dto.ExternalSystem,
                ExternalIdentifier = dto.ExternalIdentifier
            };

            await _insuranceRepository.AddAsync(insurance);
            await _unitOfWork.SaveChangesAsync();

            return insurance.Id;
        }

        public async Task UpdateAsync(InsuranceCreateEditDto dto)
        {
            if (!dto.Id.HasValue) throw new ArgumentException("Id is required for update.");

            if (await _insuranceRepository.ExistsByNameAsync(dto.Name, dto.Id.Value))
            {
                throw new ApplicationException($"Insurance with name '{dto.Name}' already exists.");
            }

            var insurance = await _insuranceRepository.GetByIdAsync(dto.Id.Value);
            if (insurance == null)
            {
                throw new ApplicationException($"Insurance with Id {dto.Id.Value} not found.");
            }

            insurance.Name = dto.Name;
            insurance.GroupId = dto.GroupId;
            insurance.PrimaryCoverage = dto.PrimaryCoverage;
            insurance.OfficeAddress = dto.OfficeAddress;
            insurance.ContactName = dto.ContactName;
            insurance.ContactNumber = dto.ContactNumber;
            insurance.ContactEmail = dto.ContactEmail;
            insurance.Remark = dto.Remark;
            insurance.IsActive = dto.IsActive;
            insurance.ExternalSystem = dto.ExternalSystem;
            insurance.ExternalIdentifier = dto.ExternalIdentifier;

            _insuranceRepository.Update(insurance);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(id);
            if (insurance != null)
            {
                _insuranceRepository.Delete(insurance);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
