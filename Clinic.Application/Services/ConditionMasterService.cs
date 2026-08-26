using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Services
{
    public class ConditionMasterService : IConditionMasterService
    {
        private readonly IConditionMasterRepository _repository;

        public ConditionMasterService(IConditionMasterRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ConditionMaster>> GetAllConditionsAsync()
        {
            var conditions = (await _repository.GetAllAsync()).ToList();

            // Apply deterministic color generation for any conditions without a color
            foreach (var condition in conditions)
            {
                if (string.IsNullOrWhiteSpace(condition.Color))
                {
                    condition.Color = GenerateColorFromName(condition.ConditionName);
                }
            }

            return conditions;
        }

        private string GenerateColorFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "#6c757d"; // Default gray

            // Deterministic hash based on the name
            int hash = 0;
            foreach (char c in name)
            {
                hash = c + ((hash << 5) - hash);
            }

            // Extract RGB components from hash
            int r = (hash & 0xFF0000) >> 16;
            int g = (hash & 0x00FF00) >> 8;
            int b = hash & 0x0000FF;

            // Adjust to favor more pleasant/visible colors
            r = (r % 128) + 64; 
            g = (g % 128) + 64;
            b = (b % 128) + 64;

            return $"#{r:X2}{g:X2}{b:X2}";
        }
    }
}
