using System;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task UpdateAsync(User user);
        Task AddAsync(User user);
    }
}
