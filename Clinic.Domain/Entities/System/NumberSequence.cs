using System;
using Clinic.Domain.Enums;

namespace Clinic.Domain.Entities.System
{
    public class NumberSequence
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string SequenceCode { get; set; } = null!;
        public string Prefix { get; set; } = string.Empty;
        
        // Date formatting if required (e.g., "yyyyMMdd").
        public string? DatePattern { get; set; }
        
        // Reset behavior independent from DatePattern
        public SequenceResetPolicy ResetPolicy { get; set; } = SequenceResetPolicy.Never;
        
        // Number of digits for padding
        public int Padding { get; set; } = 4;
        
        // Step to increment
        public int IncrementStep { get; set; } = 1;
        
        public long CurrentValue { get; set; } = 0;
        
        // Tracks the current date string used for reset logic
        public string? LastDate { get; set; }
        
        // Optimistic concurrency token
        public Guid RowVersion { get; set; } = Guid.NewGuid();

        // Standard Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
