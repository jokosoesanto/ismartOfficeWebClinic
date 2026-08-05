using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.Auth
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current Password is required.")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New Password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm Password do not match.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
