using Clinic.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class TreatmentCatalogConfiguration : IEntityTypeConfiguration<TreatmentCatalog>
    {
        public void Configure(EntityTypeBuilder<TreatmentCatalog> builder)
        {
            builder.ToTable("TreatmentCatalogs");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.TreatmentCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.TreatmentName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Description)
                .HasMaxLength(500);
                
            builder.Property(e => e.DefaultPrice)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(e => e.TreatmentCode)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Assuming a treatment name must be unique within a subcategory
            builder.HasIndex(e => new { e.SubCategoryId, e.TreatmentName })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SubCategory)
                .WithMany()
                .HasForeignKey(e => e.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ServiceType)
                .WithMany()
                .HasForeignKey(e => e.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
