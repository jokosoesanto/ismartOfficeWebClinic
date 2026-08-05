using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IChairService
    {
        Task<IEnumerable<ChairDto>> GetAllChairsAsync();
        Task<ChairDto?> GetChairByIdAsync(Guid id);
        Task SaveChairAsync(ChairDto dto, Guid? currentUserId);
        Task DeleteChairAsync(Guid id, Guid? currentUserId);
    }
}
