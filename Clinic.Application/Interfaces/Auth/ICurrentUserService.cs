using System;

namespace Clinic.Application.Interfaces.Auth
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Username { get; }
        string? IPAddress { get; }
        string? UserAgent { get; }
    }
}
