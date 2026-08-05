using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.DTOs.Auth;
using Clinic.Application.Interfaces.Auth;
using Clinic.Application.Interfaces.Configuration;

using Microsoft.AspNetCore.Authorization;

namespace Clinic.Web.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAppConfigurationService _configurationService;

        public AuthController(IAuthService authService, IAppConfigurationService configurationService)
        {
            _authService = authService;
            _configurationService = configurationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _authService.LoginAsync(model);

            if (!response.Success || response.User == null)
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Invalid login attempt.");
                return View(model);
            }

            var claims = new System.Collections.Generic.List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.User.Id.ToString()),
                new Claim(ClaimTypes.Name, response.User.Username),
                new Claim(ClaimTypes.Email, response.User.Email),
                new Claim("FullName", response.User.FullName)
            };

            foreach (var role in response.User.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var permission in response.User.Permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            if (!string.IsNullOrEmpty(response.SessionToken))
            {
                claims.Add(new Claim("SessionToken", response.SessionToken));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Read session timeout from DB, default to 30 mins
            int sessionTimeoutMinutes = await _configurationService.GetIntValueAsync("SessionTimeoutMinutes", 30);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(sessionTimeoutMinutes)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout([FromServices] IPermissionCache permissionCache)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var userId))
            {
                permissionCache.InvalidateUserPermissions(userId);
            }

            var sessionToken = User.FindFirstValue("SessionToken");
            if (!string.IsNullOrEmpty(sessionToken))
            {
                await _authService.LogoutAsync(sessionToken);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> AccessDenied([FromServices] IAuditRepository auditRepository, string? returnUrl = null)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                var role = User.FindFirstValue(ClaimTypes.Role) ?? "No Role";
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = Request.Headers["User-Agent"].ToString();

                await auditRepository.AddAsync(new Clinic.Domain.Entities.Auth.AuditLog
                {
                    UserId = userId,
                    Action = "Unauthorized Access",
                    Module = "Security",
                    NewValue = $"Attempted to access: {returnUrl}. Role: {role}",
                    IPAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = System.DateTime.UtcNow
                });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
