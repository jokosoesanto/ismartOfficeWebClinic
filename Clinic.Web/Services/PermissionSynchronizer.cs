using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinic.Domain.Entities.Auth;
using Clinic.Infrastructure.Data;

namespace Clinic.Web.Services
{
    public static class PermissionSynchronizer
    {
        public static async Task SyncAsync(AppDbContext context, Assembly assembly)
        {
            var controllerTypes = assembly.GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            var dbPermissions = await context.Permissions.ToListAsync();

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
                            Name = displayName,
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

                        if (existing.DisplayName != displayName || existing.Module != module || existing.Category != category || existing.Type != PermissionType.System)
                        {
                            existing.DisplayName = displayName;
                            existing.Name = displayName;
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
