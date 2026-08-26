using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic.Domain.Entities.MasterData;
using System;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class ConditionMasterConfiguration : IEntityTypeConfiguration<ConditionMaster>
    {
        public void Configure(EntityTypeBuilder<ConditionMaster> builder)
        {
            builder.ToTable("ConditionMasters");
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.ConditionName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(500);
            builder.Property(c => c.Color).HasMaxLength(7);

            // SEED DATA: New Web Application Master Records
            builder.HasData(
                new ConditionMaster 
                { 
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                    ConditionName = "Caries", 
                    Color = "#dc3545", // Legacy mock-up parity color
                    IsActive = true, 
                    IsDeleted = false, 
                    DisplayOrder = 1,
                    CreatedAt = new DateTime(2026, 8, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new ConditionMaster 
                { 
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                    ConditionName = "Missing", 
                    Color = "#6c757d", // Legacy mock-up parity color
                    IsActive = true, 
                    IsDeleted = false, 
                    DisplayOrder = 2,
                    CreatedAt = new DateTime(2026, 8, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new ConditionMaster 
                { 
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), 
                    ConditionName = "Fracture", 
                    Color = "#fd7e14", // Legacy mock-up parity color
                    IsActive = true, 
                    IsDeleted = false, 
                    DisplayOrder = 3,
                    CreatedAt = new DateTime(2026, 8, 24, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
