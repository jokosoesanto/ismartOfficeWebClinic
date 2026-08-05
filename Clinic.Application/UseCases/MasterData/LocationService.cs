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
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(ILocationRepository locationRepository, IAuditRepository auditRepository, IUnitOfWork unitOfWork)
        {
            _locationRepository = locationRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            return locations.Select(l => new LocationDto
            {
                Id = l.Id,
                ClinicCode = l.ClinicCode,
                ClinicName = l.ClinicName,
                City = l.City,
                Phone = l.Phone,
                OpeningTime = l.OpeningTime?.ToString(@"hh\:mm"),
                ClosingTime = l.ClosingTime?.ToString(@"hh\:mm"),
                Description = l.Description ?? string.Empty,
                IsActive = l.IsActive,
                TotalChair = l.Chairs.Count,
                AvailableChair = l.Chairs.Count(c => c.IsActive),
                OccupiedChair = 0, // Placeholder, will compute from Appointment/Session module
                MaintenanceChair = l.Chairs.Count(c => !c.IsActive) // Placeholder
            });
        }

        public async Task<LocationDto?> GetLocationByIdAsync(Guid id)
        {
            var l = await _locationRepository.GetByIdAsync(id);
            if (l == null) return null;

            return new LocationDto
            {
                Id = l.Id,
                ClinicCode = l.ClinicCode,
                ClinicName = l.ClinicName,
                Address = l.Address,
                City = l.City,
                StateProvince = l.StateProvince,
                PostalCode = l.PostalCode,
                Country = l.Country,
                Phone = l.Phone,
                Fax = l.Fax,
                Email = l.Email,
                OpeningTime = l.OpeningTime?.ToString(@"hh\:mm"),
                ClosingTime = l.ClosingTime?.ToString(@"hh\:mm"),
                Timezone = l.Timezone,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                Description = l.Description ?? string.Empty,
                IsHeadquarters = l.IsHeadquarters,
                IsActive = l.IsActive,
                TotalChair = l.Chairs.Count,
                AvailableChair = l.Chairs.Count(c => c.IsActive),
                OccupiedChair = 0, 
                MaintenanceChair = l.Chairs.Count(c => !c.IsActive)
            };
        }

        public async Task SaveLocationAsync(LocationDto dto, Guid? currentUserId)
        {
            Location? loc;
            bool isNew = false;
            string beforeValue = "null";

            if (dto.Id == Guid.Empty)
            {
                // Verify uniqueness of code
                var existing = await _locationRepository.GetByCodeAsync(dto.ClinicCode);
                if (existing != null)
                {
                    throw new InvalidOperationException("Location code must be unique.");
                }

                loc = new Location();
                loc.CreatedAt = DateTime.UtcNow;
                loc.CreatedBy = currentUserId;
                isNew = true;
            }
            else
            {
                loc = await _locationRepository.GetByIdAsync(dto.Id);
                if (loc == null) throw new Exception("Location not found");

                // Verify uniqueness of code if changed
                if (!string.Equals(loc.ClinicCode, dto.ClinicCode, StringComparison.OrdinalIgnoreCase))
                {
                    var existing = await _locationRepository.GetByCodeAsync(dto.ClinicCode);
                    if (existing != null)
                    {
                        throw new InvalidOperationException("Location code must be unique.");
                    }
                }

                beforeValue = System.Text.Json.JsonSerializer.Serialize(new { loc.ClinicCode, loc.ClinicName, loc.Description, loc.IsActive });
                loc.UpdatedAt = DateTime.UtcNow;
                loc.UpdatedBy = currentUserId;
            }

            loc.ClinicCode = dto.ClinicCode;
            loc.ClinicName = dto.ClinicName;
            loc.Address = dto.Address;
            loc.City = dto.City;
            loc.StateProvince = dto.StateProvince;
            loc.PostalCode = dto.PostalCode;
            loc.Country = dto.Country;
            loc.Phone = dto.Phone;
            loc.Fax = dto.Fax;
            loc.Email = dto.Email;
            
            if (TimeSpan.TryParse(dto.OpeningTime, out var ot)) loc.OpeningTime = ot; else loc.OpeningTime = null;
            if (TimeSpan.TryParse(dto.ClosingTime, out var ct)) loc.ClosingTime = ct; else loc.ClosingTime = null;

            loc.Timezone = dto.Timezone;
            loc.Latitude = dto.Latitude;
            loc.Longitude = dto.Longitude;
            loc.Description = dto.Description;
            loc.IsHeadquarters = dto.IsHeadquarters;
            loc.IsActive = dto.IsActive;

            if (isNew)
            {
                await _locationRepository.AddAsync(loc);
            }
            else
            {
                await _locationRepository.UpdateAsync(loc);
            }

            string afterValue = System.Text.Json.JsonSerializer.Serialize(new { loc.ClinicCode, loc.ClinicName, loc.Description, loc.IsActive });

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = isNew ? "CreateLocation" : "UpdateLocation",
                Module = "MasterData",
                EntityName = "Location",
                EntityId = loc.Id.ToString(),
                OldValue = beforeValue,
                NewValue = afterValue
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteLocationAsync(Guid id, Guid? currentUserId)
        {
            var loc = await _locationRepository.GetByIdAsync(id);
            if (loc == null) return;

            bool isUsed = await _locationRepository.HasChairsAsync(id);
            if (isUsed)
            {
                throw new InvalidOperationException("Cannot delete location because it is used by one or more chairs.");
            }

            string beforeValue = System.Text.Json.JsonSerializer.Serialize(new { loc.ClinicCode, loc.IsDeleted });
            loc.IsDeleted = true;
            loc.DeletedAt = DateTime.UtcNow;
            loc.DeletedBy = currentUserId;
            loc.IsActive = false;

            await _locationRepository.UpdateAsync(loc);

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = "DeleteLocation",
                Module = "MasterData",
                EntityName = "Location",
                EntityId = loc.Id.ToString(),
                OldValue = beforeValue,
                NewValue = System.Text.Json.JsonSerializer.Serialize(new { loc.ClinicCode, loc.IsDeleted })
            });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
