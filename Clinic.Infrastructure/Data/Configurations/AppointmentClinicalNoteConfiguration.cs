using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class AppointmentClinicalNoteConfiguration : IEntityTypeConfiguration<AppointmentClinicalNote>
    {
        public void Configure(EntityTypeBuilder<AppointmentClinicalNote> builder)
        {
            builder.ToTable("AppointmentClinicalNotes");

            builder.HasKey(e => e.Id);

            // Using HasIndex with IsUnique to enforce exactly one note record per appointment
            builder.HasIndex(e => e.AppointmentId)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0"); // Enforce uniqueness only for active records in case of soft deletes

            // 1:1 relationship with Appointment
            builder.HasOne(e => e.Appointment)
                .WithOne(a => a.ClinicalNote)
                .HasForeignKey<AppointmentClinicalNote>(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade); // If Appointment is deleted, cascade

            // Query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
