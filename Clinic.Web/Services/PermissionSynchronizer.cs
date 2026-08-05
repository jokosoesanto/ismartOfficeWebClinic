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
                new { Code = "Location.ViewAll", Name = "View All Locations", Module = "Location", Category = "Master Data" },
                new { Code = "Location.EditOwn", Name = "Edit Own Location", Module = "Location", Category = "Master Data" },
                new { Code = "Location.EditAll", Name = "Edit All Locations", Module = "Location", Category = "Master Data" },
                new { Code = "Chair.ViewAll", Name = "View All Chairs", Module = "Chair", Category = "Master Data" },
                new { Code = "Chair.EditOwn", Name = "Edit Own Chair", Module = "Chair", Category = "Master Data" },
                new { Code = "Chair.EditAll", Name = "Edit All Chairs", Module = "Chair", Category = "Master Data" },
                new { Code = "User.ViewAllLocations", Name = "View All Locations Users", Module = "User", Category = "Security" },
                new { Code = "User.ManageOwnLocation", Name = "Manage Own Location Users", Module = "User", Category = "Security" },
                new { Code = "User.ManageAllLocations", Name = "Manage All Locations Users", Module = "User", Category = "Security" }
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
                _ => "General"
            };
        }

        private static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
        }
    }
}
