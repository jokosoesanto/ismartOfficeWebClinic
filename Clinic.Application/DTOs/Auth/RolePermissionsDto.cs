namespace Clinic.Application.DTOs.Auth
{
    public class RolePermissionsDto
    {
        public System.Collections.Generic.IEnumerable<RoleDto> Roles { get; set; } = new System.Collections.Generic.List<RoleDto>();
        public System.Collections.Generic.IEnumerable<PermissionDto> Permissions { get; set; } = new System.Collections.Generic.List<PermissionDto>();
    }
}
