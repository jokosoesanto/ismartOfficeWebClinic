using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinic.Domain.Entities.Auth;
using Clinic.Infrastructure.Data;

using Microsoft.Extensions.Logging;

namespace Clinic.Web.Services
{
    public static class PermissionSynchronizer
    {
        public static async Task SyncAsync(AppDbContext context, Assembly assembly, ILogger logger)
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Administrator");
            var newPermissionsList = new List<Permission>();
            var controllerTypes = assembly.GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            var dbPermissions = await context.Permissions.ToListAsync();

            // TASK 6: LEGACY DATA MIGRATION
            bool migrated = false;
            foreach (var p in dbPermissions.Where(x => string.IsNullOrWhiteSpace(x.Code)))
            {
                p.Code = p.Name; // Map legacy Name (e.g. "Patient.View") to Code
                p.DisplayName = p.Name;
                logger.LogInformation($"[MIGRATION] Migrated legacy permission. Set Code = {p.Code}");
                context.Permissions.Update(p);
                migrated = true;
            }
            if (migrated)
            {
                await context.SaveChangesAsync();
                dbPermissions = await context.Permissions.ToListAsync(); // Refresh
            }

            // TASK 7: RUNTIME VALIDATION
            var duplicateCodes = dbPermissions.GroupBy(p => p.Code).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateCodes.Any()) logger.LogWarning($"[VALIDATION WARNING] Duplicate Codes found: {string.Join(", ", duplicateCodes)}");

            var emptyCodes = dbPermissions.Where(p => string.IsNullOrWhiteSpace(p.Code)).ToList();
            if (emptyCodes.Any()) logger.LogWarning($"[VALIDATION WARNING] Empty Codes found: {emptyCodes.Count} records");

            var invalidCodes = dbPermissions.Where(p => !string.IsNullOrWhiteSpace(p.Code) && !p.Code.Contains(".")).Select(p => p.Code).ToList();
            if (invalidCodes.Any()) logger.LogWarning($"[VALIDATION WARNING] Invalid Codes found (no dot notation): {string.Join(", ", invalidCodes)}");

            var duplicateDisplayNames = dbPermissions.Where(p => !string.IsNullOrWhiteSpace(p.DisplayName)).GroupBy(p => p.DisplayName).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateDisplayNames.Any()) logger.LogWarning($"[VALIDATION WARNING] Duplicate DisplayNames found: {string.Join(", ", duplicateDisplayNames)}");

            foreach (var controller in controllerTypes)
            {
                var controllerName = controller.Name.Replace("Controller", "");
                
                // Exclude system controllers if needed
                if (controllerName == "Home" || controllerName == "Auth") continue;

                var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                    .Where(m => m.IsPublic && !m.IsDefined(typeof(NonActionAttribute)))
                    .ToList();

                foreach (var action in actions)
                {
                    var actionName = action.Name;
                    
                    var code = $"{controllerName}.{actionName}";
                    var displayName = SplitCamelCase(actionName) + " " + SplitCamelCase(controllerName);
                    var module = controllerName;
                    var category = DetermineCategory(controllerName);

                    if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(module) || 
                        string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(displayName))
                    {
                        Console.WriteLine($"[WARNING] Skipping permission {code} due to empty fields.");
                        continue;
                    }

                    var existing = dbPermissions.FirstOrDefault(p => p.Code == code);
                    if (existing == null)
                    {
                        var newPerm = new Permission
                        {
                            Code = code,
                            Name = code,
                            DisplayName = displayName,
                            Module = module,
                            Category = category,
                            Description = $"Auto-generated permission for {code}",
                            IsActive = true,
                            Type = PermissionType.System,
                            CreatedAt = DateTime.UtcNow
                        };
                        context.Permissions.Add(newPerm);
                        dbPermissions.Add(newPerm); // Prevent duplicates in same run
                        newPermissionsList.Add(newPerm);

                        context.AuditLogs.Add(new AuditLog
                        {
                            UserId = null,
                            Action = "CreatePermission",
                            Module = "Administration",
                            EntityName = "Permission",
                            EntityId = code,
                            OldValue = "null",
                            NewValue = System.Text.Json.JsonSerializer.Serialize(new { code, displayName, module, category })
                        });
                    }
                    else
                    {
                        bool modified = false;
                        string beforeValue = System.Text.Json.JsonSerializer.Serialize(new { existing.Code, existing.DisplayName, existing.Module, existing.Category });

                        if (existing.DisplayName != displayName || existing.Module != module || existing.Category != category || existing.Type != PermissionType.System || existing.Name != code)
                        {
                            existing.DisplayName = displayName;
                            existing.Name = code;
                            existing.Module = module;
                            existing.Category = category;
                            existing.Type = PermissionType.System;
                            existing.UpdatedAt = DateTime.UtcNow;
                            modified = true;
                        }
                        if (modified)
                        {
                            context.Permissions.Update(existing);

                            context.AuditLogs.Add(new AuditLog
                            {
                                UserId = null,
                                Action = "UpdatePermission",
                                Module = "Administration",
                                EntityName = "Permission",
                                EntityId = existing.Code,
                                OldValue = beforeValue,
                                NewValue = System.Text.Json.JsonSerializer.Serialize(new { existing.Code, existing.DisplayName, existing.Module, existing.Category })
                            });
                        }
                    }
                }
            }

            var customPermissions = new[]
            {
                new { Code = "DoctorSchedule.View", Name = "View Doctor Schedule", Module = "DoctorSchedule", Category = "Master Data" },
                new { Code = "DoctorSchedule.Create", Name = "Create Doctor Schedule", Module = "DoctorSchedule", Category = "Master Data" },
                new { Code = "DoctorSchedule.Edit", Name = "Edit Doctor Schedule", Module = "DoctorSchedule", Category = "Master Data" },
                new { Code = "DoctorSchedule.Delete", Name = "Delete Doctor Schedule", Module = "DoctorSchedule", Category = "Master Data" },
                new { Code = "Location.ViewAll", Name = "View All Locations", Module = "Location", Category = "Master Data" },
                new { Code = "Location.EditOwn", Name = "Edit Own Location", Module = "Location", Category = "Master Data" },
                new { Code = "Location.EditAll", Name = "Edit All Locations", Module = "Location", Category = "Master Data" },
                new { Code = "Chair.ViewAll", Name = "View All Chairs", Module = "Chair", Category = "Master Data" },
                new { Code = "Chair.EditOwn", Name = "Edit Own Chair", Module = "Chair", Category = "Master Data" },
                new { Code = "Chair.EditAll", Name = "Edit All Chairs", Module = "Chair", Category = "Master Data" },
                new { Code = "User.ViewAllLocations", Name = "View All Locations Users", Module = "User", Category = "Security" },
                new { Code = "User.ManageOwnLocation", Name = "Manage Own Location Users", Module = "User", Category = "Security" },
                new { Code = "User.ManageAllLocations", Name = "Manage All Locations Users", Module = "User", Category = "Security" },
                new { Code = "ScheduleBoard.Export", Name = "Export Schedule Board", Module = "ScheduleBoard", Category = "Operations" },
                new { Code = "MasterReference.Import", Name = "Import Master Reference", Module = "MasterReference", Category = "System" },
                new { Code = "MasterReference.Export", Name = "Export Master Reference", Module = "MasterReference", Category = "System" },
                new { Code = "Patient.View", Name = "View Patient", Module = "Patient", Category = "Master Data" },
                new { Code = "Patient.Create", Name = "Create Patient", Module = "Patient", Category = "Master Data" },
                new { Code = "Patient.Edit", Name = "Edit Patient", Module = "Patient", Category = "Master Data" },
                new { Code = "Patient.Delete", Name = "Delete Patient", Module = "Patient", Category = "Master Data" },
                new { Code = "Patient.Export", Name = "Export Patient", Module = "Patient", Category = "Master Data" },
                new { Code = "Patient.Import", Name = "Import Patient", Module = "Patient", Category = "Master Data" },
                new { Code = "MasterData.TreatmentCategory.View", Name = "View Treatment Category", Module = "TreatmentCategory", Category = "Master Data" },
                new { Code = "MasterData.TreatmentCategory.Create", Name = "Create Treatment Category", Module = "TreatmentCategory", Category = "Master Data" },
                new { Code = "MasterData.TreatmentCategory.Edit", Name = "Edit Treatment Category", Module = "TreatmentCategory", Category = "Master Data" },
                new { Code = "MasterData.TreatmentCategory.Delete", Name = "Delete Treatment Category", Module = "TreatmentCategory", Category = "Master Data" },
                new { Code = "MasterData.TreatmentSubCategory.View", Name = "View Treatment SubCategory", Module = "TreatmentSubCategory", Category = "Master Data" },
                new { Code = "MasterData.TreatmentSubCategory.Create", Name = "Create Treatment SubCategory", Module = "TreatmentSubCategory", Category = "Master Data" },
                new { Code = "MasterData.TreatmentSubCategory.Edit", Name = "Edit Treatment SubCategory", Module = "TreatmentSubCategory", Category = "Master Data" },
                new { Code = "MasterData.TreatmentSubCategory.Delete", Name = "Delete Treatment SubCategory", Module = "TreatmentSubCategory", Category = "Master Data" },
                new { Code = "MasterData.TreatmentCatalog.View", Name = "View Treatment Catalog", Module = "TreatmentCatalog", Category = "Master Data" },
                new { Code = "MasterData.TreatmentCatalog.Create", Name = "Create Treatment Catalog", Module = "TreatmentCatalog", Category = "Master Data" },
                new { Code = "MasterData.TreatmentCatalog.Edit", Name = "Edit Treatment Catalog", Module = "TreatmentCatalog", Category = "Master Data" },
                new { Code = "MasterData.TreatmentCatalog.Delete", Name = "Delete Treatment Catalog", Module = "TreatmentCatalog", Category = "Master Data" },
                new { Code = "MasterData.Insurance.View", Name = "View Insurance", Module = "Insurance", Category = "Master Data" },
                new { Code = "MasterData.Insurance.Create", Name = "Create Insurance", Module = "Insurance", Category = "Master Data" },
                new { Code = "MasterData.Insurance.Edit", Name = "Edit Insurance", Module = "Insurance", Category = "Master Data" },
                new { Code = "MasterData.Insurance.Delete", Name = "Delete Insurance", Module = "Insurance", Category = "Master Data" }
            };

            foreach(var cp in customPermissions)
            {
                var existing = dbPermissions.FirstOrDefault(p => p.Code == cp.Code);
                if (existing == null)
                {
                    var newPerm = new Permission
                    {
                        Code = cp.Code,
                        Name = cp.Code,
                        DisplayName = cp.Name,
                        Module = cp.Module,
                        Category = cp.Category,
                        Description = $"Auto-generated permission for {cp.Code}",
                        IsActive = true,
                        Type = PermissionType.System,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.Permissions.Add(newPerm);
                    dbPermissions.Add(newPerm);
                    newPermissionsList.Add(newPerm);
                }
                else
                {
                    if (existing.DisplayName != cp.Name || existing.Module != cp.Module || existing.Category != cp.Category || existing.Type != PermissionType.System || existing.Name != cp.Code)
                    {
                        existing.DisplayName = cp.Name;
                        existing.Name = cp.Code;
                        existing.Module = cp.Module;
                        existing.Category = cp.Category;
                        existing.Type = PermissionType.System;
                        existing.UpdatedAt = DateTime.UtcNow;
                        context.Permissions.Update(existing);
                    }
                }
            }

            // Auto-assign new permissions to Administrator role
            if (adminRole != null && newPermissionsList.Any())
            {
                var existingRolePerms = await context.RolePermissions
                    .Where(rp => rp.RoleId == adminRole.Id)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();

                foreach (var newPerm in newPermissionsList)
                {
                    // Add only if not already there (shouldn't be, since it's a new permission, but safe check)
                    if (!existingRolePerms.Contains(newPerm.Id))
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = newPerm.Id
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        private static string DetermineCategory(string controllerName)
        {
            return controllerName switch
            {
                "Admin" => "Security",
                "Patient" => "Master Data",
                "Appointment" => "Transaction",
                "Billing" => "Transaction",
                "Reporting" => "Reporting",
                "MedicalRecord" => "Transaction",
                "Odontogram" => "Transaction",
                "ScheduleBoard" => "Operations",
                "MasterReference" => "System",
                "NumberSequence" => "System",
                _ => "General"
            };
        }

        private static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
        }
    }
}
