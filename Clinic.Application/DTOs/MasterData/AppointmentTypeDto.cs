using System;
using Clinic.Domain.Enums;

namespace Clinic.Application.DTOs.MasterData
{
    public class AppointmentTypeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public AppointmentComplexity Complexity { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
    }
}
