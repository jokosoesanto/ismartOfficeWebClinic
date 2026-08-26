using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IConditionMasterRepository
    {
        Task<IEnumerable<ConditionMaster>> GetAllAsync();
    }
}
