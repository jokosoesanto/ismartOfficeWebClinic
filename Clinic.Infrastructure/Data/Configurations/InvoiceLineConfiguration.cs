using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
    {
        public void Configure(EntityTypeBuilder<InvoiceLine> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(e => e.LineTotal).HasColumnType("decimal(18,2)");
            
            builder.HasOne(e => e.AppointmentTreatment)
                   .WithMany()
                   .HasForeignKey(e => e.AppointmentTreatmentId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
