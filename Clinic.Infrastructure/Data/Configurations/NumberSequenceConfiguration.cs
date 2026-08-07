using Clinic.Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class NumberSequenceConfiguration : IEntityTypeConfiguration<NumberSequence>
    {
        public void Configure(EntityTypeBuilder<NumberSequence> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.SequenceCode)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.HasIndex(x => x.SequenceCode)
                .IsUnique();

            builder.Property(x => x.Prefix)
                .HasMaxLength(20);

            builder.Property(x => x.DatePattern)
                .HasMaxLength(20);

            builder.Property(x => x.LastDate)
                .HasMaxLength(20);

            // Optimistic concurrency
            builder.Property(x => x.RowVersion)
                .IsConcurrencyToken();
        }
    }
}
