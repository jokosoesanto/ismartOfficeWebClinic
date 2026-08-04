using System;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Auth;
using Clinic.Domain.Entities.Auth;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.Auth
{
    public class AuditRepository : IAuditRepository
    {
        private readonly AppDbContext _context;

        public AuditRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
        }
    }
}
