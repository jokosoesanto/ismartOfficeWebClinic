using System;

namespace Clinic.Application.DTOs.MasterData
{
    public class ChairDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
    }
}
