using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PaymentAllocationConfiguration : IEntityTypeConfiguration<PaymentAllocation>
    {
        public void Configure(EntityTypeBuilder<PaymentAllocation> builder)
        {
            builder.ToTable("PaymentAllocations");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.AllocatedAmount).HasColumnType("decimal(18,2)");
            builder.HasQueryFilter(e => !e.IsDeleted);

            builder.HasOne(e => e.Payment)
                .WithMany() // Payment entity doesn't have a collection of Allocations yet, keeping it simple.
                .HasForeignKey(e => e.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.InvoiceLine)
                .WithMany()
                .HasForeignKey(e => e.InvoiceLineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
