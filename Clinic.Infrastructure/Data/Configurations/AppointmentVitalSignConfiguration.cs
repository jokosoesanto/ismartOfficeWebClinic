using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class AppointmentVitalSignConfiguration : IEntityTypeConfiguration<AppointmentVitalSign>
    {
        public void Configure(EntityTypeBuilder<AppointmentVitalSign> builder)
        {
            builder.ToTable("AppointmentVitalSigns");

            builder.HasKey(e => e.Id);

            // Using HasIndex with IsUnique to enforce exactly one vital sign record per appointment
            builder.HasIndex(e => e.AppointmentId)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0"); // Enforce uniqueness only for active records in case of soft deletes

            // 1:1 relationship with Appointment
            builder.HasOne(e => e.Appointment)
                .WithOne(a => a.VitalSign)
                .HasForeignKey<AppointmentVitalSign>(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade); // If Appointment is deleted, cascade

            builder.Property(e => e.Temperature)
                .HasColumnType("decimal(4,1)"); // E.g., 36.5

            // Query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
