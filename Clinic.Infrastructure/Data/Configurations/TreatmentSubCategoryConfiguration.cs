using Clinic.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class TreatmentSubCategoryConfiguration : IEntityTypeConfiguration<TreatmentSubCategory>
    {
        public void Configure(EntityTypeBuilder<TreatmentSubCategory> builder)
        {
            builder.ToTable("TreatmentSubCategories");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.SubCategoryCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.SubCategoryName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            // SubCategoryCode unique per active records
            builder.HasIndex(e => e.SubCategoryCode)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // SubCategoryName unique per active records within a Category
            builder.HasIndex(e => new { e.CategoryId, e.SubCategoryName })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
