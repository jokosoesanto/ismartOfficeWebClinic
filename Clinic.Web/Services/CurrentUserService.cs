using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Clinic.Application.Interfaces.Auth;

namespace Clinic.Web.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return val != null ? Guid.Parse(val) : null;
            }
        }

        public string? Username => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        public string? IPAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        public string? UserAgent => _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
    }
}
