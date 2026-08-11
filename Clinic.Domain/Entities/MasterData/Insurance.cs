using System;
using Clinic.Domain.Entities.System;

namespace Clinic.Domain.Entities.MasterData
{
    public class Insurance
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string Name { get; set; } = null!;
        public Guid GroupId { get; set; }
        public MasterReference Group { get; set; } = null!;
        
        public string PrimaryCoverage { get; set; } = null!;
        public string? OfficeAddress { get; set; }
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactEmail { get; set; }
        public string? Remark { get; set; }
        
        public bool IsActive { get; set; } = true;

        // Future B2B Integration Boundary
        public string? ExternalSystem { get; set; }
        public string? ExternalIdentifier { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
