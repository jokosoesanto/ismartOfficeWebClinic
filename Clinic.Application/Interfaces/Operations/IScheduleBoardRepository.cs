using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IScheduleBoardRepository
    {
        Task<IEnumerable<ScheduleBoardDto>> GetSchedulesAsync(
            Guid? locationId, 
            Guid? doctorId, 
            Guid? specialtyId, 
            int? dayOfWeek,
            DateTime? specificDate,
            string? searchKeyword);
            
        Task<IEnumerable<ScheduleBoardDto>> GetAllSchedulesForDataTablesAsync();
    }
}
