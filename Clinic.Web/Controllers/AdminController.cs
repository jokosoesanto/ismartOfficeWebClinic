using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly Clinic.Application.Interfaces.Auth.IAuthService _authService;

        public AdminController(Clinic.Application.Interfaces.Auth.IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return GetAdminView("System Administration");
        }

        [HttpGet("Users")]
        public IActionResult Users()
        {
            return GetAdminView("Manage Users");
        }

        [HttpGet("Roles")]
        public async System.Threading.Tasks.Task<IActionResult> Roles()
        {
            var roles = await _authService.GetRolesAsync();
            return GetAdminView("Manage Roles", roles);
        }

        [HttpGet("RoleForm/{id?}")]
        public async System.Threading.Tasks.Task<IActionResult> RoleForm(Guid? id = null)
        {
            Clinic.Application.DTOs.Auth.RoleDto? role = null;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                var roles = await _authService.GetRolesAsync();
                role = roles.FirstOrDefault(r => r.Id == id.Value);
            }
            
            var metadata = new UIMetadata 
            { 
                Title = id.HasValue && id.Value != Guid.Empty ? "Edit Role" : "Add Role", 
                ModuleName = "Admin", 
                Mode = RenderingMode.Template,
                Data = role
            };
            return View("Templates/Admin_RoleForm", metadata);
        }

        [HttpPost("SaveRole")]
        public async System.Threading.Tasks.Task<IActionResult> SaveRole([FromForm] Clinic.Application.DTOs.Auth.RoleDto roleDto)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _authService.SaveRoleAsync(roleDto, currentUserId);
            return RedirectToAction("Roles");
        }

        [HttpPost("DeleteRole/{id}")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteRole(Guid id)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _authService.DeleteRoleAsync(id, currentUserId);
            return RedirectToAction("Roles");
        }

        [HttpGet("Permissions")]
        public async System.Threading.Tasks.Task<IActionResult> Permissions()
        {
            var perms = await _authService.GetAllPermissionsAsync();
            return GetAdminView("Manage Permissions", perms);
        }

        [HttpGet("PermissionForm/{id?}")]
        public async System.Threading.Tasks.Task<IActionResult> PermissionForm(Guid? id = null)
        {
            Clinic.Application.DTOs.Auth.PermissionDto? permission = null;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                var perms = await _authService.GetAllPermissionsAsync();
                permission = perms.FirstOrDefault(p => p.Id == id.Value);
            }
            
            var metadata = new UIMetadata 
            { 
                Title = id.HasValue && id.Value != Guid.Empty ? "Edit Permission" : "Add Permission", 
                ModuleName = "Admin", 
                Mode = RenderingMode.Template,
                Data = permission
            };
            return View("Templates/Admin_PermissionForm", metadata);
        }

        [HttpPost("SavePermission")]
        public async System.Threading.Tasks.Task<IActionResult> SavePermission([FromForm] Clinic.Application.DTOs.Auth.PermissionDto permissionDto)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _authService.SavePermissionAsync(permissionDto, currentUserId);
            return RedirectToAction("Permissions");
        }

        [HttpGet("AssignPermissions/{roleId}")]
        public async System.Threading.Tasks.Task<IActionResult> AssignPermissions(string roleId)
        {
            // For now, pass all roles and all permissions so the UI can filter by roleId
            var data = new Clinic.Application.DTOs.Auth.RolePermissionsDto
            {
                Roles = await _authService.GetRolesAsync(),
                Permissions = await _authService.GetAllPermissionsAsync()
            };
            ViewBag.SelectedRoleId = roleId;
            var metadata = new UIMetadata { Title = "Assign Permissions", ModuleName = "Admin", Mode = RenderingMode.Template, Data = data };
            return View("Templates/Admin_RolePermissionAssignment", metadata);
        }

        [HttpPost("SaveRolePermissions")]
        public async System.Threading.Tasks.Task<IActionResult> SaveRolePermissions(string roleId, [FromForm] System.Collections.Generic.List<Guid> permissionIds)
        {
            if (Guid.TryParse(roleId, out var id))
            {
                await _authService.AssignRolePermissionsAsync(id, permissionIds);
            }
            return RedirectToAction("Roles");
        }

        [HttpGet("Locations")]
        public IActionResult Locations()
        {
            return GetAdminView("Manage Locations");
        }

        [HttpGet("Chairs")]
        public IActionResult Chairs()
        {
            return GetAdminView("Manage Chairs");
        }

        [HttpGet("Doctors")]
        public IActionResult Doctors()
        {
            return GetAdminView("Manage Doctors");
        }

        [HttpGet("Insurance")]
        public IActionResult Insurance()
        {
            return GetAdminView("Manage Insurance");
        }

        [HttpGet("Procedures")]
        public IActionResult Procedures()
        {
            return GetAdminView("Manage Procedures");
        }

        [HttpGet("Configuration")]
        public IActionResult Configuration()
        {
            return GetAdminView("Configuration");
        }

        [HttpGet("Lookup")]
        public IActionResult Lookup()
        {
            return GetAdminView("Master Lookup");
        }

        private IActionResult GetAdminView(string title, object? data = null)
        {
            var metadata = new UIMetadata
            {
                Title = title,
                ModuleName = "Admin",
                Mode = RenderingMode.Template,
                Data = data
            };
            return View("Templates/Admin_List", metadata);
        }

        [HttpGet("CreateUser")]
        public async System.Threading.Tasks.Task<IActionResult> CreateUser()
        {
            var metadata = new UIMetadata 
            { 
                Title = "Add User", 
                ModuleName = "Admin", 
                Mode = RenderingMode.Template,
                Data = await _authService.GetRolesAsync()
            };
            return View("Templates/Admin_UserForm", metadata);
        }

        [HttpGet("CreateProvider")]
        public IActionResult CreateProvider()
        {
            var metadata = new UIMetadata { Title = "Add Provider", ModuleName = "Admin", Mode = RenderingMode.Template };
            return View("Templates/Admin_ProviderForm", metadata);
        }

        [HttpGet("UserDetails/{id}")]
        public IActionResult UserDetails(string id)
        {
            var metadata = new UIMetadata { Title = "User Detail", ModuleName = "Admin", Mode = RenderingMode.Template };
            return View("Templates/Admin_UserDetail", metadata);
        }
    }
}
