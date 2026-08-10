using Clinic.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class TreatmentCategoryConfiguration : IEntityTypeConfiguration<TreatmentCategory>
    {
        public void Configure(EntityTypeBuilder<TreatmentCategory> builder)
        {
            builder.ToTable("TreatmentCategories");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.CategoryCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            // CategoryCode unique per active records
            builder.HasIndex(e => e.CategoryCode)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // CategoryName unique per active records
            builder.HasIndex(e => e.CategoryName)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
