using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class AppointmentDiagnosisConfiguration : IEntityTypeConfiguration<AppointmentDiagnosis>
    {
        public void Configure(EntityTypeBuilder<AppointmentDiagnosis> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.HasQueryFilter(e => !e.IsDeleted);

            builder.Property(e => e.Remark)
                .HasMaxLength(1000);

            builder.HasOne(e => e.Appointment)
                .WithMany(a => a.Diagnoses)
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.DiagnosisMaster)
                .WithMany()
                .HasForeignKey(e => e.DiagnosisMasterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
