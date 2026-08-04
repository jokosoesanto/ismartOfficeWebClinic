using Clinic.Domain.Entities.Auth;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        string HashPassword(User user, string password);
        bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
    }
}
