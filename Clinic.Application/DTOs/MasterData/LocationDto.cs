using System;

namespace Clinic.Application.DTOs.MasterData
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string ClinicCode { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? StateProvince { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public string? OpeningTime { get; set; } // Can be string representation
        public string? ClosingTime { get; set; } // Can be string representation
        public string? Timezone { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsHeadquarters { get; set; }
        public bool IsActive { get; set; } = true;
        
        public int TotalChair { get; set; }
        public int AvailableChair { get; set; }
        public int OccupiedChair { get; set; }
        public int MaintenanceChair { get; set; }
    }
}
