using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");

            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(e => e.Appointment)
                .WithMany()
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.PrescribedByUser)
                .WithMany()
                .HasForeignKey(e => e.PrescribedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Items)
                .WithOne(e => e.Prescription)
                .HasForeignKey(e => e.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
