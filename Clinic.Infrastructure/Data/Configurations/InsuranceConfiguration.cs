using Clinic.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class InsuranceConfiguration : IEntityTypeConfiguration<Insurance>
    {
        public void Configure(EntityTypeBuilder<Insurance> builder)
        {
            builder.ToTable("Insurances");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.PrimaryCoverage)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(e => e.OfficeAddress)
                .HasMaxLength(500);

            builder.Property(e => e.ContactName)
                .HasMaxLength(100);

            builder.Property(e => e.ContactNumber)
                .HasMaxLength(50);

            builder.Property(e => e.ContactEmail)
                .HasMaxLength(150);

            builder.Property(e => e.Remark)
                .HasMaxLength(1000);

            builder.Property(e => e.ExternalSystem)
                .HasMaxLength(100);

            builder.Property(e => e.ExternalIdentifier)
                .HasMaxLength(100);

            builder.HasOne(e => e.Group)
                .WithMany()
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Name unique per active records
            builder.HasIndex(e => e.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
