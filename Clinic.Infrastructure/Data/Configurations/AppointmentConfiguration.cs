using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(e => e.Id);

            // Persist Enum as string
            builder.Property(e => e.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(e => e.Notes).HasMaxLength(1000);

            // Relationships
            builder.HasOne(e => e.Patient)
                   .WithMany()
                   .HasForeignKey(e => e.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Doctor)
                   .WithMany()
                   .HasForeignKey(e => e.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Location)
                   .WithMany()
                   .HasForeignKey(e => e.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Chair)
                   .WithMany()
                   .HasForeignKey(e => e.ChairId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
