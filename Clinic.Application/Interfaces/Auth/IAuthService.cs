using System.Threading.Tasks;
using Clinic.Application.DTOs.Auth;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task LogoutAsync(string sessionToken);
        Task<bool> ChangePasswordAsync(string username, ChangePasswordDto request);
        Task<UserDto?> GetCurrentUserProfileAsync();
        Task<System.Collections.Generic.IEnumerable<RoleDto>> GetRolesAsync();
        Task<System.Collections.Generic.IEnumerable<PermissionDto>> GetAllPermissionsAsync();
        Task AssignRolePermissionsAsync(Guid roleId, System.Collections.Generic.List<Guid> permissionIds);
        Task SaveRoleAsync(RoleDto roleDto, Guid? currentUserId);
        Task SavePermissionAsync(PermissionDto dto, Guid? currentUserId);
        Task DeleteRoleAsync(Guid roleId, Guid? currentUserId);
    }
}
