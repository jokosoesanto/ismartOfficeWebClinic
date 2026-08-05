using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.DTOs.MasterData;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Auth;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.Auth;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.UseCases.MasterData
{
    public class ChairService : IChairService
    {
        private readonly IChairRepository _chairRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChairService(IChairRepository chairRepository, IAuditRepository auditRepository, IUnitOfWork unitOfWork)
        {
            _chairRepository = chairRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ChairDto>> GetAllChairsAsync()
        {
            var chairs = await _chairRepository.GetAllAsync();
            return chairs.Select(c => new ChairDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description ?? string.Empty,
                IsActive = c.IsActive,
                LocationId = c.LocationId,
                LocationName = c.Location?.ClinicName ?? string.Empty
            });
        }

        public async Task<ChairDto?> GetChairByIdAsync(Guid id)
        {
            var c = await _chairRepository.GetByIdAsync(id);
            if (c == null) return null;

            return new ChairDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description ?? string.Empty,
                IsActive = c.IsActive,
                LocationId = c.LocationId,
                LocationName = c.Location?.ClinicName ?? string.Empty
            };
        }

        public async Task SaveChairAsync(ChairDto dto, Guid? currentUserId)
        {
            Chair? chair;
            bool isNew = false;
            string beforeValue = "null";

            if (dto.Id == Guid.Empty)
            {
                // Verify uniqueness of code
                var existing = await _chairRepository.GetByCodeAsync(dto.Code);
                if (existing != null)
                {
                    throw new InvalidOperationException("Chair code must be unique.");
                }

                chair = new Chair();
                chair.CreatedAt = DateTime.UtcNow;
                chair.CreatedBy = currentUserId;
                isNew = true;
            }
            else
            {
                chair = await _chairRepository.GetByIdAsync(dto.Id);
                if (chair == null) throw new Exception("Chair not found");

                // Verify uniqueness of code if changed
                if (!string.Equals(chair.Code, dto.Code, StringComparison.OrdinalIgnoreCase))
                {
                    var existing = await _chairRepository.GetByCodeAsync(dto.Code);
                    if (existing != null)
                    {
                        throw new InvalidOperationException("Chair code must be unique.");
                    }
                }

                beforeValue = System.Text.Json.JsonSerializer.Serialize(new { chair.Code, chair.Name, chair.LocationId, chair.Description, chair.IsActive });
                chair.UpdatedAt = DateTime.UtcNow;
                chair.UpdatedBy = currentUserId;
            }

            chair.Code = dto.Code;
            chair.Name = dto.Name;
            chair.LocationId = dto.LocationId;
            chair.Description = dto.Description;
            chair.IsActive = dto.IsActive;

            if (isNew)
            {
                await _chairRepository.AddAsync(chair);
            }
            else
            {
                await _chairRepository.UpdateAsync(chair);
            }

            string afterValue = System.Text.Json.JsonSerializer.Serialize(new { chair.Code, chair.Name, chair.LocationId, chair.Description, chair.IsActive });

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = isNew ? "CreateChair" : "UpdateChair",
                Module = "MasterData",
                EntityName = "Chair",
                EntityId = chair.Id.ToString(),
                OldValue = beforeValue,
                NewValue = afterValue
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteChairAsync(Guid id, Guid? currentUserId)
        {
            var chair = await _chairRepository.GetByIdAsync(id);
            if (chair == null) return;

            bool isUsed = await _chairRepository.HasAppointmentsAsync(id);
            if (isUsed)
            {
                throw new InvalidOperationException("Cannot delete chair because it is used by one or more appointments.");
            }

            string beforeValue = System.Text.Json.JsonSerializer.Serialize(new { chair.Code, chair.IsDeleted });
            chair.IsDeleted = true;
            chair.DeletedAt = DateTime.UtcNow;
            chair.DeletedBy = currentUserId;
            chair.IsActive = false;

            await _chairRepository.UpdateAsync(chair);

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = "DeleteChair",
                Module = "MasterData",
                EntityName = "Chair",
                EntityId = chair.Id.ToString(),
                OldValue = beforeValue,
                NewValue = System.Text.Json.JsonSerializer.Serialize(new { chair.Code, chair.IsDeleted })
            });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
