using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.DTOs.Auth;
using Clinic.Application.Interfaces.Auth;

using Microsoft.AspNetCore.Authorization;

namespace Clinic.Web.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Note: If you stored the session token in a claim, you'd retrieve it here to revoke.
            // For simplicity, we just revoke the cookie.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
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
