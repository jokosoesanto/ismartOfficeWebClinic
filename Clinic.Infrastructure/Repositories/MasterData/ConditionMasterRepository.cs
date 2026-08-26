using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.MasterData
{
    public class ConditionMasterRepository : IConditionMasterRepository
    {
        private readonly AppDbContext _context;

        public ConditionMasterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConditionMaster>> GetAllAsync()
        {
            return await _context.ConditionMasters
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.ConditionName)
                .ToListAsync();
        }
    }
}
