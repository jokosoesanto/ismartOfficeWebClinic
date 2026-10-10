using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PatientGuarantorConfiguration : IEntityTypeConfiguration<PatientGuarantor>
    {
        public void Configure(EntityTypeBuilder<PatientGuarantor> builder)
        {
            builder.ToTable("PatientGuarantors");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Relationship)
                .HasMaxLength(100);

            builder.Property(e => e.Phone)
                .HasMaxLength(100);

            builder.Property(e => e.Email)
                .HasMaxLength(150);

            builder.Property(e => e.Address)
                .HasMaxLength(500);

            builder.HasOne(e => e.Patient)
                .WithMany(p => p.Guarantors)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
