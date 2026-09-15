using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class AppointmentFollowUpConfiguration : IEntityTypeConfiguration<AppointmentFollowUp>
    {
        public void Configure(EntityTypeBuilder<AppointmentFollowUp> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Reason).HasMaxLength(1000);

            builder.HasOne(x => x.Appointment)
                   .WithMany()
                   .HasForeignKey(x => x.AppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Patient)
                   .WithMany()
                   .HasForeignKey(x => x.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
