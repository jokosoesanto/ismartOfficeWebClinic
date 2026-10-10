using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class InvoiceAdjustmentConfiguration : IEntityTypeConfiguration<InvoiceAdjustment>
    {
        public void Configure(EntityTypeBuilder<InvoiceAdjustment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AdjustmentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Reason)
                .HasMaxLength(500);

            builder.HasOne(x => x.Invoice)
                .WithMany(x => x.Adjustments)
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
