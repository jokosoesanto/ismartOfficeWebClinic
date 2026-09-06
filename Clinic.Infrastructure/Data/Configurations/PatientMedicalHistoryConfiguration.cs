using Clinic.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PatientMedicalHistoryConfiguration : IEntityTypeConfiguration<PatientMedicalHistory>
    {
        public void Configure(EntityTypeBuilder<PatientMedicalHistory> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Condition).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Notes).HasMaxLength(500);

            builder.HasOne(e => e.Patient)
                .WithMany(p => p.SystemicDiseases)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
