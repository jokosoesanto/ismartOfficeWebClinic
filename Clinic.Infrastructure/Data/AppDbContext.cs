using Microsoft.EntityFrameworkCore;
using Clinic.Domain.Entities;
using Clinic.Domain.Entities.Auth;
using Clinic.Infrastructure.Data.Configurations;

namespace Clinic.Infrastructure.Data
{
    public class AppDbContext : DbContext, Clinic.Application.Interfaces.IUnitOfWork
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<SystemSetting> SystemSettings { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.System.MasterReference> MasterReferences { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.System.NumberSequence> NumberSequences { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<UserSession> UserSessions { get; set; } = null!;
        public DbSet<UserLocation> UserLocations { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.MasterData.Location> Locations { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.MasterData.Chair> Chairs { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.MasterData.Specialty> Specialties { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.MasterData.Doctor> Doctors { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.MasterData.DoctorLocation> DoctorLocations { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.MasterData.DoctorSchedule> DoctorSchedules { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.Configuration.AppConfiguration> AppConfigurations { get; set; } = null!;
        public DbSet<Clinic.Domain.Entities.System.FileMetadata> FileMetadatas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.FileMetadataConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.NumberSequenceConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.MasterReferenceConfiguration());

            modelBuilder.Entity<Clinic.Domain.Entities.Configuration.AppConfiguration>(entity =>
            {
                entity.ToTable("AppConfigurations");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Category, e.Key }).IsUnique();
            });

            modelBuilder.Entity<Clinic.Domain.Entities.MasterData.DoctorLocation>(entity =>
            {
                entity.HasKey(e => new { e.DoctorId, e.LocationId });
            });

            modelBuilder.Entity<Clinic.Domain.Entities.MasterData.Doctor>(entity =>
            {
                entity.HasIndex(e => e.DoctorCode).IsUnique();
            });

            modelBuilder.Entity<Clinic.Domain.Entities.MasterData.Specialty>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
            
            // Explicitly set max length for string to avoid NTEXT/TEXT
            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Value).HasMaxLength(500).IsRequired();
            });

            AuthConfiguration.Apply(modelBuilder);
        }

        public override int SaveChanges()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int entityCount = ChangeTracker.Entries().Count(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);
            string caller = new System.Diagnostics.StackTrace().GetFrame(1)?.GetMethod()?.Name ?? "Unknown";
            
            try
            {
                int result = base.SaveChanges();
                sw.Stop();
                System.Console.WriteLine($"[{DateTime.UtcNow:O}] [SAVE_CHANGES] Thread: {Environment.CurrentManagedThreadId}, Hash: {this.GetHashCode()}, Caller: {caller}, Entities: {entityCount}, Elapsed: {sw.ElapsedMilliseconds}ms");
                return result;
            }
            catch (System.Exception ex)
            {
                sw.Stop();
                System.Console.WriteLine($"[{DateTime.UtcNow:O}] [SAVE_CHANGES_ERROR] Thread: {Environment.CurrentManagedThreadId}, Hash: {this.GetHashCode()}, Caller: {caller}, Entities: {entityCount}, Elapsed: {sw.ElapsedMilliseconds}ms, Exception: {ex.Message}");
                throw;
            }
        }

        public override async System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int entityCount = ChangeTracker.Entries().Count(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);
            
            string caller = "UnknownAsync";
            var st = new System.Diagnostics.StackTrace();
            foreach (var frame in st.GetFrames())
            {
                var method = frame.GetMethod();
                if (method != null && method.DeclaringType != null && method.DeclaringType.FullName != null && !method.DeclaringType.FullName.Contains("System.Runtime.CompilerServices") && !method.DeclaringType.FullName.Contains("AppDbContext"))
                {
                    caller = $"{method.DeclaringType.Name}.{method.Name}";
                    break;
                }
            }
            
            try
            {
                int result = await base.SaveChangesAsync(cancellationToken);
                sw.Stop();
                System.Console.WriteLine($"[{DateTime.UtcNow:O}] [SAVE_CHANGES_ASYNC] Thread: {Environment.CurrentManagedThreadId}, Hash: {this.GetHashCode()}, Caller: {caller}, Entities: {entityCount}, Elapsed: {sw.ElapsedMilliseconds}ms");
                return result;
            }
            catch (System.Exception ex)
            {
                sw.Stop();
                System.Console.WriteLine($"[{DateTime.UtcNow:O}] [SAVE_CHANGES_ASYNC_ERROR] Thread: {Environment.CurrentManagedThreadId}, Hash: {this.GetHashCode()}, Caller: {caller}, Entities: {entityCount}, Elapsed: {sw.ElapsedMilliseconds}ms, Exception: {ex.Message}");
                throw;
            }
        }
    }
}

