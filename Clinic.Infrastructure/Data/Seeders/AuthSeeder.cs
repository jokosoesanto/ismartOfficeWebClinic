using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Clinic.Domain.Entities.Auth;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Data.Seeders
{
    public static class AuthSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Users.Any()) return; // DB has been seeded

            var hasher = new PasswordHasher<User>();
            var now = DateTime.UtcNow;

            // 1. Create Permissions
            var permissions = new List<Permission>
            {
                new() { Id = Guid.NewGuid(), Name = "Patient.View", Description = "View patient records" },
                new() { Id = Guid.NewGuid(), Name = "Patient.Create", Description = "Create new patient" },
                new() { Id = Guid.NewGuid(), Name = "Appointment.Edit", Description = "Edit appointments" },
                new() { Id = Guid.NewGuid(), Name = "Billing.Process", Description = "Process billing" },
                new() { Id = Guid.NewGuid(), Name = "Administration.Users", Description = "Manage users" },
            };

            context.Permissions.AddRange(permissions);

            // 2. Create Roles
            var adminRole = new Role { Id = Guid.NewGuid(), Name = "Administrator", Description = "System Administrator", IsSystem = true, CreatedAt = now };
            var doctorRole = new Role { Id = Guid.NewGuid(), Name = "Doctor", Description = "Doctor", IsSystem = true, CreatedAt = now };
            var nurseRole = new Role { Id = Guid.NewGuid(), Name = "Nurse", Description = "Nurse", IsSystem = true, CreatedAt = now };
            var receptionistRole = new Role { Id = Guid.NewGuid(), Name = "Receptionist", Description = "Receptionist", IsSystem = true, CreatedAt = now };
            var cashierRole = new Role { Id = Guid.NewGuid(), Name = "Cashier", Description = "Cashier", IsSystem = true, CreatedAt = now };
            var managerRole = new Role { Id = Guid.NewGuid(), Name = "Manager", Description = "Clinic Manager", IsSystem = true, CreatedAt = now };

            context.Roles.AddRange(adminRole, doctorRole, nurseRole, receptionistRole, cashierRole, managerRole);

            // 3. Role-Permission mapping
            foreach (var p in permissions)
            {
                context.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = p.Id });
            }

            // 4. Create default admin user
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                NormalizedUsername = "ADMIN",
                FullName = "System Administrator",
                Email = "admin@clinic.local",
                NormalizedEmail = "ADMIN@CLINIC.LOCAL",
                IsActive = true,
                CreatedAt = now
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin");

            context.Users.Add(adminUser);

            // 5. User-Role mapping
            context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });

            context.SaveChanges();
        }
    }
}
