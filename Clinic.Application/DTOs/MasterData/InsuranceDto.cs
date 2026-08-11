using System;

namespace Clinic.Application.DTOs.MasterData
{
    public class InsuranceDto
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = null!;
        public Guid GroupId { get; set; }
        public string? GroupName { get; set; }
        
        public string PrimaryCoverage { get; set; } = null!;
        public string? OfficeAddress { get; set; }
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactEmail { get; set; }
        public string? Remark { get; set; }
        
        public bool IsActive { get; set; }

        public string? ExternalSystem { get; set; }
        public string? ExternalIdentifier { get; set; }
    }
}
