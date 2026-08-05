using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.DTOs.Auth;
using Clinic.Application.Interfaces.Auth;
using Clinic.Application.UI;

namespace Clinic.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;

        public ProfileController(IAuthService authService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var profile = await _authService.GetCurrentUserProfileAsync();
            if (profile == null) return NotFound();

            var updateDto = new UpdateProfileDto
            {
                FullName = profile.FullName,
                DisplayName = profile.DisplayName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber
            };

            var uiMetadata = new UIMetadata
            {
                ModuleName = "Profile",
                Title = "My Profile",
                Mode = RenderingMode.Template,
                Data = updateDto
            };

            return View("Templates/Profile_Edit", uiMetadata);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProfileDto model)
        {
            if (!ModelState.IsValid)
            {
                var uiMetadata = new UIMetadata { ModuleName = "Profile", Title = "My Profile", Mode = RenderingMode.Template, Data = model };
                return View("Templates/Profile_Edit", uiMetadata);
            }

            var userId = _currentUserService.UserId;
            if (userId == null) return Unauthorized();

            try
            {
                await _authService.UpdateProfileAsync(userId.Value, model);
                TempData["SuccessMessage"] = "Profile updated successfully. Note: you may need to logout and login again for some changes to take effect in the menu.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var uiMetadata = new UIMetadata { ModuleName = "Profile", Title = "My Profile", Mode = RenderingMode.Template, Data = model };
                return View("Templates/Profile_Edit", uiMetadata);
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var uiMetadata = new UIMetadata
            {
                ModuleName = "Profile",
                Title = "Change Password",
                Mode = RenderingMode.Template,
                Data = new ChangePasswordDto()
            };

            return View("Templates/Profile_ChangePassword", uiMetadata);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                var uiMetadata = new UIMetadata { ModuleName = "Profile", Title = "Change Password", Mode = RenderingMode.Template, Data = model };
                return View("Templates/Profile_ChangePassword", uiMetadata);
            }

            var username = _currentUserService.Username;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(username, model);

            if (result)
            {
                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Current password is incorrect.");
            var errorMetadata = new UIMetadata { ModuleName = "Profile", Title = "Change Password", Mode = RenderingMode.Template, Data = model };
            return View("Templates/Profile_ChangePassword", errorMetadata);
        }
    }
}
