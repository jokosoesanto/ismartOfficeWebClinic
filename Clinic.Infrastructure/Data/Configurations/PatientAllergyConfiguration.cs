using Clinic.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PatientAllergyConfiguration : IEntityTypeConfiguration<PatientAllergy>
    {
        public void Configure(EntityTypeBuilder<PatientAllergy> builder)
        {
            builder.ToTable("PatientAllergies");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Allergen)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.Severity)
                   .HasMaxLength(50);

            builder.Property(e => e.Notes)
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne(e => e.Patient)
                   .WithMany(p => p.Allergies)
                   .HasForeignKey(e => e.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
