using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class AppointmentTypeConfiguration : IEntityTypeConfiguration<AppointmentType>
    {
        public void Configure(EntityTypeBuilder<AppointmentType> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(x => x.DisplayName)
                .HasMaxLength(100);
                
            builder.Property(x => x.Description)
                .HasMaxLength(500);

            // Global Query Filter for soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);

            // Relationships
            builder.HasMany(x => x.Appointments)
                .WithOne(a => a.AppointmentType)
                .HasForeignKey(a => a.AppointmentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
