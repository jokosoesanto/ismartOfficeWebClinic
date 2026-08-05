using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.MasterData
{
    public class Location
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClinicCode { get; set; } = null!;
        public string ClinicName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? StateProvince { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public string? Timezone { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Description { get; set; }
        public bool IsHeadquarters { get; set; }
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public ICollection<Chair> Chairs { get; set; } = new List<Chair>();
        public ICollection<Clinic.Domain.Entities.Auth.User> Users { get; set; } = new List<Clinic.Domain.Entities.Auth.User>();
        public ICollection<Clinic.Domain.Entities.Auth.UserLocation> UserLocations { get; set; } = new List<Clinic.Domain.Entities.Auth.UserLocation>();
    }
}
