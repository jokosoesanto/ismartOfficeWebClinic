using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PatientCreditConfiguration : IEntityTypeConfiguration<PatientCredit>
    {
        public void Configure(EntityTypeBuilder<PatientCredit> builder)
        {
            builder.ToTable("PatientCredits");
            
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
                
            builder.Property(c => c.Reason)
                .HasMaxLength(250);

            builder.HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
