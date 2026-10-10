using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class InventoryBatchConfiguration : IEntityTypeConfiguration<InventoryBatch>
    {
        public void Configure(EntityTypeBuilder<InventoryBatch> builder)
        {
            builder.ToTable("InventoryBatches");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.BatchNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(e => e.InventoryItem)
                .WithMany(i => i.InventoryBatches)
                .HasForeignKey(e => e.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique batch number per item
            builder.HasIndex(e => new { e.InventoryItemId, e.BatchNumber }).IsUnique();

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
