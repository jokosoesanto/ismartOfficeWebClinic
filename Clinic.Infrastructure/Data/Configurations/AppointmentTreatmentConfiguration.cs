using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class AppointmentTreatmentConfiguration : IEntityTypeConfiguration<AppointmentTreatment>
    {
        public void Configure(EntityTypeBuilder<AppointmentTreatment> builder)
        {
            builder.ToTable("AppointmentTreatments");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.ActualPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.SiteNumber)
                .HasMaxLength(50);

            builder.Property(e => e.SiteDetail)
                .HasMaxLength(50);

            builder.Property(e => e.Remark)
                .HasMaxLength(1000);

            builder.HasOne(e => e.Appointment)
                .WithMany()
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.TreatmentItem)
                .WithMany()
                .HasForeignKey(e => e.TreatmentItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
