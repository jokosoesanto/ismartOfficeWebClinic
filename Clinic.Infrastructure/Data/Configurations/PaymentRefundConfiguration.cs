using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PaymentRefundConfiguration : IEntityTypeConfiguration<PaymentRefund>
    {
        public void Configure(EntityTypeBuilder<PaymentRefund> builder)
        {
            builder.ToTable("PaymentRefunds");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(r => r.Reason)
                .HasMaxLength(255);

            builder.HasOne(r => r.Payment)
                .WithMany(p => p.Refunds)
                .HasForeignKey(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust to Restrict if project prefers
        }
    }
}
