using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Clinic.Web.Controllers
{
    [Authorize(Policy = "Admin.Index")]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly Clinic.Application.Interfaces.Auth.IAuthService _authService;
        private readonly Clinic.Application.Interfaces.MasterData.ILocationService _locationService;
        private readonly Clinic.Application.Interfaces.MasterData.IChairService _chairService;

        public AdminController(
            Clinic.Application.Interfaces.Auth.IAuthService authService,
            Clinic.Application.Interfaces.MasterData.ILocationService locationService,
            Clinic.Application.Interfaces.MasterData.IChairService chairService)
        {
            _authService = authService;
            _locationService = locationService;
            _chairService = chairService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return GetAdminView("System Administration");
        }

        [HttpGet("Users")]
        [Authorize(Policy = "Admin.Users")]
        public async System.Threading.Tasks.Task<IActionResult> Users()
        {
            var users = await _authService.GetAllUsersAsync();
            return GetAdminView("Manage Users", users);
        }

        [HttpGet("Roles")]
        [Authorize(Policy = "Admin.Roles")]
        public async System.Threading.Tasks.Task<IActionResult> Roles()
        {
            var roles = await _authService.GetRolesAsync();
            return GetAdminView("Manage Roles", roles);
        }

        [HttpGet("RoleForm/{id?}")]
        [Authorize(Policy = "Admin.RoleForm")]
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
        [Authorize(Policy = "Admin.SaveRole")]
        public async System.Threading.Tasks.Task<IActionResult> SaveRole([FromForm] Clinic.Application.DTOs.Auth.RoleDto roleDto)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _authService.SaveRoleAsync(roleDto, currentUserId);
            return RedirectToAction("Roles");
        }

        [HttpPost("DeleteRole/{id}")]
        [Authorize(Policy = "Admin.DeleteRole")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteRole(Guid id)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _authService.DeleteRoleAsync(id, currentUserId);
            return RedirectToAction("Roles");
        }

        [HttpGet("Permissions")]
        [Authorize(Policy = "Admin.Permissions")]
        public async System.Threading.Tasks.Task<IActionResult> Permissions()
        {
            var perms = await _authService.GetAllPermissionsAsync();
            return GetAdminView("Manage Permissions", perms);
        }

        [HttpGet("PermissionForm/{id?}")]
        [Authorize(Policy = "Admin.PermissionForm")]
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
        [Authorize(Policy = "Admin.SavePermission")]
        public async System.Threading.Tasks.Task<IActionResult> SavePermission([FromForm] Clinic.Application.DTOs.Auth.PermissionDto permissionDto)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _authService.SavePermissionAsync(permissionDto, currentUserId);
            return RedirectToAction("Permissions");
        }

        [HttpGet("AssignPermissions/{roleId}")]
        [Authorize(Policy = "Admin.AssignPermissions")]
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
        [Authorize(Policy = "Admin.SaveRolePermissions")]
        public async System.Threading.Tasks.Task<IActionResult> SaveRolePermissions(string roleId, [FromForm] System.Collections.Generic.List<Guid> permissionIds)
        {
            if (Guid.TryParse(roleId, out var id))
            {
                await _authService.AssignRolePermissionsAsync(id, permissionIds);
            }
            return RedirectToAction("Roles");
        }

        [HttpGet("Locations")]
        [Authorize(Policy = "Admin.Locations")]
        public async System.Threading.Tasks.Task<IActionResult> Locations()
        {
            var data = await _locationService.GetAllLocationsAsync();
            return GetAdminView("Manage Locations", data);
        }

        [HttpGet("LocationForm/{id?}")]
        [Authorize(Policy = "Admin.LocationForm")]
        public async System.Threading.Tasks.Task<IActionResult> LocationForm(Guid? id = null)
        {
            Clinic.Application.DTOs.MasterData.LocationDto? location = null;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                location = await _locationService.GetLocationByIdAsync(id.Value);
            }
            
            var metadata = new UIMetadata 
            { 
                Title = id.HasValue && id.Value != Guid.Empty ? "Edit Location" : "Add Location", 
                ModuleName = "Admin", 
                Mode = RenderingMode.Template,
                Data = location
            };
            return View("Templates/Admin_LocationForm", metadata);
        }

        [HttpPost("SaveLocation")]
        [Authorize(Policy = "Admin.SaveLocation")]
        public async System.Threading.Tasks.Task<IActionResult> SaveLocation([FromForm] Clinic.Application.DTOs.MasterData.LocationDto locationDto)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _locationService.SaveLocationAsync(locationDto, currentUserId);
            return RedirectToAction("Locations");
        }

        [HttpPost("DeleteLocation/{id}")]
        [Authorize(Policy = "Admin.DeleteLocation")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteLocation(Guid id)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _locationService.DeleteLocationAsync(id, currentUserId);
            return RedirectToAction("Locations");
        }

        [HttpGet("Chairs")]
        [Authorize(Policy = "Admin.Chairs")]
        public async System.Threading.Tasks.Task<IActionResult> Chairs()
        {
            var data = await _chairService.GetAllChairsAsync();
            return GetAdminView("Manage Chairs", data);
        }

        [HttpGet("ChairForm/{id?}")]
        [Authorize(Policy = "Admin.ChairForm")]
        public async System.Threading.Tasks.Task<IActionResult> ChairForm(Guid? id = null)
        {
            Clinic.Application.DTOs.MasterData.ChairDto? chair = null;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                chair = await _chairService.GetChairByIdAsync(id.Value);
            }
            
            var metadata = new UIMetadata 
            { 
                Title = id.HasValue && id.Value != Guid.Empty ? "Edit Chair" : "Add Chair", 
                ModuleName = "Admin", 
                Mode = RenderingMode.Template,
                Data = new { Chair = chair, Locations = await _locationService.GetAllLocationsAsync() }
            };
            return View("Templates/Admin_ChairForm", metadata);
        }

        [HttpPost("SaveChair")]
        [Authorize(Policy = "Admin.SaveChair")]
        public async System.Threading.Tasks.Task<IActionResult> SaveChair([FromForm] Clinic.Application.DTOs.MasterData.ChairDto chairDto)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _chairService.SaveChairAsync(chairDto, currentUserId);
            return RedirectToAction("Chairs");
        }

        [HttpPost("DeleteChair/{id}")]
        [Authorize(Policy = "Admin.DeleteChair")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteChair(Guid id)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _chairService.DeleteChairAsync(id, currentUserId);
            return RedirectToAction("Chairs");
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

        [HttpGet("UserForm/{id?}")]
        [Authorize(Policy = "Admin.UserForm")]
        public async System.Threading.Tasks.Task<IActionResult> UserForm(Guid? id = null)
        {
            Clinic.Application.DTOs.Auth.UserDto? user = null;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                user = await _authService.GetUserByIdAsync(id.Value);
            }

            var metadata = new UIMetadata 
            { 
                Title = id.HasValue && id.Value != Guid.Empty ? "Edit User" : "Add User", 
                ModuleName = "Admin", 
                Mode = RenderingMode.Template,
                Data = new { User = user, Roles = await _authService.GetRolesAsync(), Locations = await _locationService.GetAllLocationsAsync() }
            };
            return View("Templates/Admin_UserForm", metadata);
        }

        [HttpPost("SaveUser")]
        [Authorize(Policy = "Admin.SaveUser")]
        public async System.Threading.Tasks.Task<IActionResult> SaveUser([FromForm] Clinic.Application.DTOs.Auth.UserDto userDto, [FromForm] string? newPassword)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _authService.SaveUserAsync(userDto, newPassword, currentUserId);
            return RedirectToAction("Users");
        }

        [HttpPost("DeleteUser/{id}")]
        [Authorize(Policy = "Admin.DeleteUser")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteUser(Guid id)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _authService.DeleteUserAsync(id, currentUserId);
            return RedirectToAction("Users");
        }

        [HttpGet("CreateProvider")]
        public IActionResult CreateProvider()
        {
            var metadata = new UIMetadata { Title = "Add Provider", ModuleName = "Admin", Mode = RenderingMode.Template };
            return View("Templates/Admin_ProviderForm", metadata);
        }

        [HttpGet("UserDetails/{id}")]
        public async System.Threading.Tasks.Task<IActionResult> UserDetails(Guid id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            var metadata = new UIMetadata { Title = "User Detail", ModuleName = "Admin", Mode = RenderingMode.Template, Data = user };
            return View("Templates/Admin_UserDetail", metadata);
        }
    }
}
