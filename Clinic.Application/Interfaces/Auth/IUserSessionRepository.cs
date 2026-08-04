using System;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IUserSessionRepository
    {
        Task AddAsync(UserSession session);
        Task RevokeSessionAsync(string sessionToken);
        Task<UserSession?> GetByTokenAsync(string sessionToken);
    }
}
