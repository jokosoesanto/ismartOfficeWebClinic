namespace Clinic.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SessionToken { get; set; }
        public UserDto? User { get; set; }
    }
}
