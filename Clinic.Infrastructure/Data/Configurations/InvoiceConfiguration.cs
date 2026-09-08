using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(e => e.Id);
            
            // Unique Invoice Number
            builder.HasIndex(e => e.InvoiceNumber).IsUnique();
            
            // 1 Appointment = 1 Invoice rule (Duplicate Billing Protection)
            builder.HasIndex(e => e.AppointmentId).IsUnique().HasFilter("\"IsDeleted\" = 0");
            
            builder.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            
            builder.HasMany(e => e.InvoiceLines)
                   .WithOne(e => e.Invoice)
                   .HasForeignKey(e => e.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
