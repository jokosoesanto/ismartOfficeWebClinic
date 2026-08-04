using Microsoft.AspNetCore.Identity;
using Clinic.Application.Interfaces.Auth;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Infrastructure.Security
{
    public class IdentityPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<User> _hasher;

        public IdentityPasswordHasher()
        {
            _hasher = new PasswordHasher<User>();
        }

        public string HashPassword(User user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result != PasswordVerificationResult.Failed;
        }
    }
}
