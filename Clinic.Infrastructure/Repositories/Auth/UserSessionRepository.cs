using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.Auth;
using Clinic.Domain.Entities.Auth;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.Auth
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly AppDbContext _context;

        public UserSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserSession session)
        {
            await _context.UserSessions.AddAsync(session);
        }

        public async Task<UserSession?> GetByTokenAsync(string sessionToken)
        {
            return await _context.UserSessions.FirstOrDefaultAsync(s => s.SessionToken == sessionToken);
        }

        public async Task RevokeSessionAsync(string sessionToken)
        {
            var session = await GetByTokenAsync(sessionToken);
            if (session != null)
            {
                session.RevokedAt = DateTime.UtcNow;
                _context.UserSessions.Update(session);
            }
        }
    }
}
