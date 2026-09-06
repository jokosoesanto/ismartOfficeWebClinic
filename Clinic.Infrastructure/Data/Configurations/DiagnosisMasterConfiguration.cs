using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class DiagnosisMasterConfiguration : IEntityTypeConfiguration<DiagnosisMaster>
    {
        public void Configure(EntityTypeBuilder<DiagnosisMaster> builder)
        {
            builder.ToTable("DiagnosisMasters");
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.DiagnosisCode).IsRequired().HasMaxLength(50);
            builder.Property(e => e.DiagnosisName).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).HasMaxLength(1000);

            builder.HasIndex(e => e.DiagnosisCode).IsUnique().HasFilter("\"IsDeleted\" = 0");
        }
    }
}
