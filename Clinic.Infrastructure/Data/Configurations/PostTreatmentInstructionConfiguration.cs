using Clinic.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data.Configurations
{
    public class PostTreatmentInstructionConfiguration : IEntityTypeConfiguration<PostTreatmentInstruction>
    {
        public void Configure(EntityTypeBuilder<PostTreatmentInstruction> builder)
        {
            builder.ToTable("PostTreatmentInstructions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InstructionText)
                .IsRequired();

            // Relationships
            builder.HasOne(x => x.Appointment)
                .WithMany()
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Patient)
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Audit
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
