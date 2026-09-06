using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class AppointmentChiefComplaintConfiguration : IEntityTypeConfiguration<AppointmentChiefComplaint>
    {
        public void Configure(EntityTypeBuilder<AppointmentChiefComplaint> builder)
        {
            builder.ToTable("AppointmentChiefComplaints");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Complaint)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Notes)
                .HasMaxLength(1000);

            builder.Property(e => e.ToothNumber)
                .HasMaxLength(10);

            builder.HasOne(e => e.Appointment)
                .WithMany(a => a.ChiefComplaints)
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade); // If Appointment is deleted, delete complaints

            // Query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
