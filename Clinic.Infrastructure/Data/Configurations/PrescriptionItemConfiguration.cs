using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems");

            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.MedicationName)
                .HasMaxLength(200)
                .IsRequired();
                
            builder.Property(e => e.Dosage)
                .HasMaxLength(100)
                .IsRequired();
                
            builder.Property(e => e.Frequency)
                .HasMaxLength(100)
                .IsRequired();
                
            builder.Property(e => e.Instruction)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
