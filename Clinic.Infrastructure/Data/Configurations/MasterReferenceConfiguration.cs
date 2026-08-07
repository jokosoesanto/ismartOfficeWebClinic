using Clinic.Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class MasterReferenceConfiguration : IEntityTypeConfiguration<MasterReference>
    {
        public void Configure(EntityTypeBuilder<MasterReference> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            // Compound index for Category and Code, ensure unique
            builder.HasIndex(x => new { x.Category, x.Code })
                .IsUnique();

            // Self-referencing relationship for hierarchy
            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Global query filter for soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
