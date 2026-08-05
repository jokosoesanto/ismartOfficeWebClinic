using Microsoft.EntityFrameworkCore;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Infrastructure.Data.Configurations
{
    public static class AuthConfiguration
    {
        public static void Apply(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.NormalizedUsername).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NormalizedEmail).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Salt).HasMaxLength(100);
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.DisplayName).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasIndex(e => e.NormalizedUsername).IsUnique();
                entity.HasIndex(e => e.NormalizedEmail).IsUnique();
                
                entity.HasQueryFilter(e => !e.IsDeleted);
                
                entity.HasOne(u => u.PrimaryLocation).WithMany(l => l.Users).HasForeignKey(u => u.PrimaryLocationId).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<UserLocation>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LocationId });
                entity.HasOne(ul => ul.User).WithMany(u => u.UserAccessibleLocations).HasForeignKey(ul => ul.UserId);
                entity.HasOne(ul => ul.Location).WithMany(l => l.UserLocations).HasForeignKey(ul => ul.LocationId);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);

                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);

                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
                entity.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PermissionId });
                entity.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId);
                entity.HasOne(rp => rp.Permission).WithMany(p => p.RolePermissions).HasForeignKey(rp => rp.PermissionId);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Module).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EntityName).HasMaxLength(100);
                entity.Property(e => e.EntityId).HasMaxLength(100);
            });

            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SessionToken).HasMaxLength(255).IsRequired();
                entity.HasOne(s => s.User).WithMany(u => u.Sessions).HasForeignKey(s => s.UserId);
                entity.HasIndex(e => e.SessionToken).IsUnique();
            });
        }
    }
}
