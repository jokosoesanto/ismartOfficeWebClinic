using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class TreatmentConsentConfiguration : IEntityTypeConfiguration<TreatmentConsent>
    {
        public void Configure(EntityTypeBuilder<TreatmentConsent> builder)
        {
            builder.ToTable("TreatmentConsents");
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.ConsentedByUser)
                .WithMany()
                .HasForeignKey(e => e.ConsentedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}