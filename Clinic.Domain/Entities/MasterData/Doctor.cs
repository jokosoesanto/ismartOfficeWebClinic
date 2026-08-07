using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.MasterData
{
    public class Doctor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DoctorCode { get; set; } = null!;
        public string? Title { get; set; }
        public string FullName { get; set; } = null!;
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        
        public Guid SpecialtyId { get; set; }
        public Specialty Specialty { get; set; } = null!;

        public string? LicenseNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Photo { get; set; }
        public string? Signature { get; set; }

        public Guid? PrimaryLocationId { get; set; }
        public Location? PrimaryLocation { get; set; }

        public int ConsultationDuration { get; set; } = 30;
        public int AppointmentInterval { get; set; } = 15;
        public string? Color { get; set; }
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
        public ICollection<DoctorLocation> DoctorLocations { get; set; } = new List<DoctorLocation>();
    }
}