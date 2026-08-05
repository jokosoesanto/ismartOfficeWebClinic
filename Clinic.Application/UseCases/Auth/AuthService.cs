using System;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Auth;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Auth;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Application.UseCases.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IUserSessionRepository sessionRepository,
            IAuditRepository auditRepository,
            IPasswordHasher passwordHasher,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _sessionRepository = sessionRepository;
            _auditRepository = auditRepository;
            _passwordHasher = passwordHasher;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            
            if (user == null || !user.IsActive)
            {
                await LogFailedAttempt(user?.Id, "Invalid username or inactive account");
                await _unitOfWork.SaveChangesAsync();
                return new AuthResponseDto { Success = false, ErrorMessage = "Invalid credentials." };
            }

            if (user.IsLocked && user.LockoutUntil > DateTime.UtcNow)
            {
                await LogFailedAttempt(user.Id, "Account locked");
                await _unitOfWork.SaveChangesAsync();
                return new AuthResponseDto { Success = false, ErrorMessage = "Account is locked." };
            }

            var isPasswordValid = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (!isPasswordValid)
            {
                user.FailedLoginCount++;
                if (user.FailedLoginCount >= 5) // Example threshold
                {
                    user.IsLocked = true;
                    user.LockoutUntil = DateTime.UtcNow.AddMinutes(15);
                }
                await _userRepository.UpdateAsync(user);
                await LogFailedAttempt(user.Id, "Invalid password");
                await _unitOfWork.SaveChangesAsync();
                return new AuthResponseDto { Success = false, ErrorMessage = "Invalid credentials." };
            }

            // Success
            user.FailedLoginCount = 0;
            user.IsLocked = false;
            user.LockoutUntil = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            var sessionToken = Guid.NewGuid().ToString("N");
            var session = new UserSession
            {
                UserId = user.Id,
                SessionToken = sessionToken,
                IPAddress = _currentUserService.IPAddress,
                UserAgent = _currentUserService.UserAgent,
                ExpiresAt = request.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(8)
            };

            await _sessionRepository.AddAsync(session);

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "Login",
                Module = "Authentication",
                IPAddress = _currentUserService.IPAddress,
                UserAgent = _currentUserService.UserAgent
            });

            await _unitOfWork.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                MustChangePassword = user.MustChangePassword,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Permissions = user.UserRoles.SelectMany(ur => ur.Role.RolePermissions).Select(rp => rp.Permission.Code).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList()
            };

            return new AuthResponseDto
            {
                Success = true,
                SessionToken = sessionToken,
                User = userDto
            };
        }

        public async Task LogoutAsync(string sessionToken)
        {
            if (string.IsNullOrEmpty(sessionToken)) return;

            var session = await _sessionRepository.GetByTokenAsync(sessionToken);
            if (session != null)
            {
                session.RevokedAt = DateTime.UtcNow;
                await _sessionRepository.RevokeSessionAsync(sessionToken);

                await _auditRepository.AddAsync(new AuditLog
                {
                    UserId = session.UserId,
                    Action = "Logout",
                    Module = "Authentication",
                    IPAddress = _currentUserService.IPAddress,
                    UserAgent = _currentUserService.UserAgent
                });
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ChangePasswordAsync(string username, ChangePasswordDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return false;

            if (!_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword))
                return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.LastPasswordChangedAt = DateTime.UtcNow;
            user.MustChangePassword = false;
            user.PermissionVersion = Guid.NewGuid().ToString("N");
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "ChangePassword",
                Module = "Authentication",
                IPAddress = _currentUserService.IPAddress,
                UserAgent = _currentUserService.UserAgent
            });

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<UserDto?> GetCurrentUserProfileAsync()
        {
            var userId = _currentUserService.UserId;
            if (userId == null) return null;

            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                MustChangePassword = user.MustChangePassword,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Permissions = user.UserRoles.SelectMany(ur => ur.Role.RolePermissions).Select(rp => rp.Permission.Name).Distinct().ToList()
            };
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");

            string beforeValue = System.Text.Json.JsonSerializer.Serialize(new { user.FullName, user.DisplayName, user.Email, user.PhoneNumber });

            user.FullName = dto.FullName;
            user.DisplayName = dto.DisplayName;
            user.Email = dto.Email;
            user.NormalizedEmail = dto.Email.ToUpperInvariant();
            user.PhoneNumber = dto.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = userId,
                Action = "UpdateProfile",
                Module = "Authentication",
                EntityName = "User",
                EntityId = userId.ToString(),
                OldValue = beforeValue,
                NewValue = System.Text.Json.JsonSerializer.Serialize(new { user.FullName, user.DisplayName, user.Email, user.PhoneNumber }),
                IPAddress = _currentUserService.IPAddress,
                UserAgent = _currentUserService.UserAgent
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<System.Collections.Generic.IEnumerable<RoleDto>> GetRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Where(r => !r.IsDeleted).Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description ?? string.Empty,
                IsSystem = r.IsSystem,
                IsActive = r.IsActive,
                IsDeleted = r.IsDeleted,
                UsersCount = r.UserRoles?.Count ?? 0,
                PermissionsCount = r.RolePermissions?.Count(rp => rp.Permission != null && rp.Permission.IsActive) ?? 0,
                PermissionIds = r.RolePermissions?.Where(rp => rp.Permission != null && rp.Permission.IsActive).Select(rp => rp.PermissionId).ToList() ?? new System.Collections.Generic.List<System.Guid>()
            });
        }

        public async Task<System.Collections.Generic.IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            var perms = await _permissionRepository.GetAllAsync();
            return perms.Where(p => p.IsActive).Select(p => new PermissionDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                DisplayName = p.DisplayName,
                Category = p.Category,
                Module = p.Module,
                IsActive = p.IsActive,
                Type = p.Type,
                Description = p.Description ?? string.Empty
            });
        }

        public async Task AssignRolePermissionsAsync(Guid roleId, System.Collections.Generic.List<Guid> permissionIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) throw new Exception("Role not found");

            var allActivePerms = await _permissionRepository.GetAllAsync();
            var activePermIds = allActivePerms.Where(p => p.IsActive).Select(p => p.Id).ToHashSet();

            role.RolePermissions.Clear();
            foreach (var pid in permissionIds)
            {
                if (activePermIds.Contains(pid))
                {
                    role.RolePermissions.Add(new Clinic.Domain.Entities.Auth.RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = pid
                    });
                }
            }
            await _roleRepository.UpdateAsync(role);

            var affectedUsers = await _userRepository.GetUsersByRoleIdAsync(roleId);
            foreach (var u in affectedUsers)
            {
                u.PermissionVersion = Guid.NewGuid().ToString("N");
                await _userRepository.UpdateAsync(u);
            }

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = null, // Will use ICurrentUserService internally if configured, or pass explicitly if modified
                Action = "AssignPermission",
                Module = "Administration",
                EntityName = "Role",
                EntityId = roleId.ToString(),
                OldValue = "[]",
                NewValue = System.Text.Json.JsonSerializer.Serialize(permissionIds)
            });
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task LogFailedAttempt(Guid? userId, string details)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = "LoginFailed",
                Module = "Authentication",
                OldValue = null,
                NewValue = details
            };
            await _auditRepository.AddAsync(auditLog);
        }

        public async Task SaveRoleAsync(RoleDto roleDto, Guid? currentUserId)
        {
            Role? role;
            bool isNew = false;
            string beforeValue = "null";

            if (roleDto.Id == Guid.Empty)
            {
                role = new Role();
                role.CreatedAt = DateTime.UtcNow;
                role.CreatedBy = currentUserId;
                isNew = true;
            }
            else
            {
                role = await _roleRepository.GetByIdAsync(roleDto.Id);
                if (role == null) throw new Exception("Role not found");
                beforeValue = System.Text.Json.JsonSerializer.Serialize(new { role.Name, role.Description, role.IsActive });
                role.UpdatedAt = DateTime.UtcNow;
                role.UpdatedBy = currentUserId;
            }

            if (isNew)
            {
                if (string.Equals(roleDto.Name, "Administrator", StringComparison.OrdinalIgnoreCase) && !roleDto.IsActive)
                {
                    roleDto.IsActive = true;
                }
            }
            else
            {
                if (string.Equals(role.Name, "Administrator", StringComparison.OrdinalIgnoreCase))
                {
                    if (!roleDto.IsActive)
                    {
                        await _auditRepository.AddAsync(new AuditLog
                        {
                            UserId = currentUserId,
                            Action = "Attempt to deactivate Administrator Role",
                            Module = "Administration",
                            EntityName = "Role",
                            EntityId = role.Id.ToString(),
                            OldValue = beforeValue,
                            NewValue = "Rejected"
                        });
                        await _unitOfWork.SaveChangesAsync();
                        throw new Exception("Administrator role cannot be deactivated.");
                    }
                    roleDto.Name = role.Name; // Prevent rename just in case
                }
            }

            role.Name = roleDto.Name;
            role.Description = roleDto.Description;
            role.IsActive = roleDto.IsActive;
            
            if (isNew)
            {
                await _roleRepository.AddAsync(role);
            }
            else
            {
                await _roleRepository.UpdateAsync(role);
                var affectedUsers = await _userRepository.GetUsersByRoleIdAsync(role.Id);
                foreach (var u in affectedUsers)
                {
                    u.PermissionVersion = Guid.NewGuid().ToString("N");
                    await _userRepository.UpdateAsync(u);
                }
            }

            string afterValue = System.Text.Json.JsonSerializer.Serialize(new { role.Name, role.Description, role.IsActive });

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = isNew ? "CreateRole" : "UpdateRole",
                Module = "Administration",
                EntityName = "Role",
                EntityId = role.Id.ToString(),
                OldValue = beforeValue,
                NewValue = afterValue
            });
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SavePermissionAsync(PermissionDto dto, Guid? currentUserId)
        {
            var perm = await _permissionRepository.GetByIdAsync(dto.Id);
            if (perm == null) throw new Exception("Permission not found");

            string beforeValue = System.Text.Json.JsonSerializer.Serialize(new { perm.Description, perm.IsActive });

            perm.Description = dto.Description;
            perm.IsActive = dto.IsActive;
            perm.UpdatedAt = DateTime.UtcNow;

            await _permissionRepository.UpdateAsync(perm);
            
            // Because a permission's status changed, it might affect any role/user holding it.
            // Simplest safe approach: invalidate everyone's permission version.
            await _userRepository.UpdateAllPermissionVersionsAsync(Guid.NewGuid().ToString("N"));

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = "UpdatePermission",
                Module = "Administration",
                EntityName = "Permission",
                EntityId = perm.Id.ToString(),
                OldValue = beforeValue,
                NewValue = System.Text.Json.JsonSerializer.Serialize(new { perm.Description, perm.IsActive })
            });
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(Guid roleId, Guid? currentUserId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null || role.IsSystem) return;

            string beforeValue = System.Text.Json.JsonSerializer.Serialize(new { role.Name, role.IsDeleted });
            
            role.IsDeleted = true;
            role.DeletedAt = DateTime.UtcNow;
            role.DeletedBy = currentUserId;
            role.IsActive = false;

            await _roleRepository.UpdateAsync(role);

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = "DeleteRole",
                Module = "Administration",
                EntityName = "Role",
                EntityId = role.Id.ToString(),
                OldValue = beforeValue,
                NewValue = System.Text.Json.JsonSerializer.Serialize(new { role.Name, role.IsDeleted })
            });
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<System.Collections.Generic.IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                DisplayName = u.DisplayName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Notes = u.Notes,
                MustChangePassword = u.MustChangePassword,
                IsActive = u.IsActive,
                PrimaryLocationId = u.PrimaryLocationId,
                PrimaryLocationName = u.PrimaryLocation?.ClinicName ?? string.Empty,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList(),
                AccessibleLocationIds = u.UserAccessibleLocations.Select(ul => ul.LocationId).ToList()
            });
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var u = await _userRepository.GetByIdAsync(id);
            if (u == null) return null;

            return new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                DisplayName = u.DisplayName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Notes = u.Notes,
                MustChangePassword = u.MustChangePassword,
                IsActive = u.IsActive,
                PrimaryLocationId = u.PrimaryLocationId,
                PrimaryLocationName = u.PrimaryLocation?.ClinicName ?? string.Empty,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList(),
                AccessibleLocationIds = u.UserAccessibleLocations.Select(ul => ul.LocationId).ToList()
            };
        }

        public async Task SaveUserAsync(UserDto dto, string? newPassword, Guid? currentUserId)
        {
            User? user;
            bool isNew = false;
            string beforeValue = "null";

            if (dto.Id == Guid.Empty)
            {
                var existing = await _userRepository.GetByUsernameAsync(dto.Username);
                if (existing != null) throw new InvalidOperationException("Username must be unique.");

                user = new User();
                user.CreatedAt = DateTime.UtcNow;
                isNew = true;
            }
            else
            {
                user = await _userRepository.GetByIdAsync(dto.Id);
                if (user == null) throw new Exception("User not found");

                if (!string.Equals(user.Username, dto.Username, StringComparison.OrdinalIgnoreCase))
                {
                    var existing = await _userRepository.GetByUsernameAsync(dto.Username);
                    if (existing != null) throw new InvalidOperationException("Username must be unique.");
                }

                beforeValue = System.Text.Json.JsonSerializer.Serialize(new { user.Username, user.FullName, user.Email, user.IsActive, user.PrimaryLocationId });
                user.UpdatedAt = DateTime.UtcNow;
            }

            user.Username = dto.Username;
            user.NormalizedUsername = dto.Username.ToUpperInvariant();
            user.FullName = dto.FullName;
            user.DisplayName = dto.DisplayName;
            user.Email = dto.Email;
            user.NormalizedEmail = dto.Email.ToUpperInvariant();
            user.PhoneNumber = dto.PhoneNumber;
            user.Notes = dto.Notes;
            user.IsActive = dto.IsActive;
            user.MustChangePassword = dto.MustChangePassword;
            user.PrimaryLocationId = dto.PrimaryLocationId;
            user.PermissionVersion = Guid.NewGuid().ToString("N");
            
            if (isNew)
            {
                if (string.IsNullOrWhiteSpace(newPassword)) throw new InvalidOperationException("Password is required for new users.");
                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            }
            else if (!string.IsNullOrWhiteSpace(newPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            }

            user.UserRoles.Clear();
            foreach (var roleId in dto.RoleIds)
            {
                user.UserRoles.Add(new UserRole { RoleId = roleId });
            }

            user.UserAccessibleLocations.Clear();
            foreach (var locId in dto.AccessibleLocationIds)
            {
                user.UserAccessibleLocations.Add(new UserLocation { LocationId = locId });
            }

            if (isNew) await _userRepository.AddAsync(user);
            else await _userRepository.UpdateAsync(user);

            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = isNew ? "CreateUser" : "UpdateUser",
                Module = "Administration",
                EntityName = "User",
                EntityId = user.Id.ToString(),
                OldValue = beforeValue,
                NewValue = System.Text.Json.JsonSerializer.Serialize(new { user.Username, user.FullName, user.Email, user.IsActive, user.PrimaryLocationId })
            });
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id, Guid? currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return;
            
            if (user.UserRoles.Any(ur => string.Equals(ur.Role.Name, "Administrator", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Administrator user cannot be deleted.");
            }

            string beforeValue = System.Text.Json.JsonSerializer.Serialize(new { user.Username, user.IsActive, user.IsDeleted });
            user.IsActive = false; // Soft delete / Deactivate
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.DeletedBy = currentUserId;
            
            await _userRepository.UpdateAsync(user);
            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = "DeleteUser",
                Module = "Administration",
                EntityName = "User",
                EntityId = user.Id.ToString(),
                OldValue = beforeValue,
                NewValue = System.Text.Json.JsonSerializer.Serialize(new { user.Username, user.IsActive })
            });
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
