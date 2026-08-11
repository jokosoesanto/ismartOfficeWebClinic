using System;

namespace Clinic.Application.DTOs.MasterData
{
    public class InsuranceListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public string PrimaryCoverage { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public bool IsActive { get; set; }
        public string? ExternalIdentifier { get; set; }
    }
}
