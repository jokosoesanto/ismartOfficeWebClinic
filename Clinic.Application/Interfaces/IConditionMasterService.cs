using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces
{
    public interface IConditionMasterService
    {
        Task<IEnumerable<ConditionMaster>> GetAllConditionsAsync();
    }
}
