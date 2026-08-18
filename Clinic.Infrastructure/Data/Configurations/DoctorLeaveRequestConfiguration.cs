using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class DoctorLeaveRequestConfiguration : IEntityTypeConfiguration<DoctorLeaveRequest>
    {
        public void Configure(EntityTypeBuilder<DoctorLeaveRequest> builder)
        {
            builder.ToTable("DoctorLeaveRequests");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Reason).HasMaxLength(1000);

            // Relationship to Doctor
            builder.HasOne(e => e.Doctor)
                   .WithMany()
                   .HasForeignKey(e => e.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1:N relationship to DoctorLeaveDate
            builder.HasMany(e => e.LeaveDates)
                   .WithOne(e => e.DoctorLeaveRequest)
                   .HasForeignKey(e => e.DoctorLeaveRequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class DoctorLeaveDateConfiguration : IEntityTypeConfiguration<DoctorLeaveDate>
    {
        public void Configure(EntityTypeBuilder<DoctorLeaveDate> builder)
        {
            builder.ToTable("DoctorLeaveDates");
            builder.HasKey(e => e.Id);

            // Unique constraint: prevent duplicate date per request
            builder.HasIndex(e => new { e.DoctorLeaveRequestId, e.Date }).IsUnique();

            builder.Property(e => e.CancellationReason).HasMaxLength(500);
        }
    }
}
