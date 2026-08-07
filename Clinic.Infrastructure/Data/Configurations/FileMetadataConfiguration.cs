using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.System;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
    {
        public void Configure(EntityTypeBuilder<FileMetadata> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(500);
            builder.Property(x => x.StoredFileName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.RelativePath).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Extension).IsRequired().HasMaxLength(20);
            builder.Property(x => x.MimeType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ContentHash).IsRequired().HasMaxLength(64);
            
            // Store enum as string for readability
            builder.Property(x => x.Module).HasConversion<string>().HasMaxLength(50);

            // Indexes for searching / querying
            builder.HasIndex(x => new { x.Module, x.EntityId });
            builder.HasIndex(x => x.ContentHash);
            
            // Soft delete query filter
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
