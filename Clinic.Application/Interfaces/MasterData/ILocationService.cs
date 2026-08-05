using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        Task<LocationDto?> GetLocationByIdAsync(Guid id);
        Task SaveLocationAsync(LocationDto dto, Guid? currentUserId);
        Task DeleteLocationAsync(Guid id, Guid? currentUserId);
    }
}
