using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReceiptNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.ReceiptNumber)
                .IsUnique();

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ReferenceNumber)
                .HasMaxLength(100);

            builder.Property(x => x.Notes)
                .HasMaxLength(500);

            builder.HasOne(x => x.Invoice)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
