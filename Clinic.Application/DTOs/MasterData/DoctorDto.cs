using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class DoctorDto
    {
        public Guid? Id { get; set; }
        [Required] public string DoctorCode { get; set; } = null!;
        public string? Title { get; set; }
        [Required] public string FullName { get; set; } = null!;
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        
        [Required] public Guid SpecialtyId { get; set; }
        public string? LicenseNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Photo { get; set; }
        public string? Signature { get; set; }

        public Guid? PrimaryLocationId { get; set; }
        public int ConsultationDuration { get; set; } = 30;
        public int AppointmentInterval { get; set; } = 15;
        public string? Color { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;

        public List<DoctorScheduleDto> Schedules { get; set; } = new();
    }

    public class DoctorScheduleDto
    {
        public Guid? Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid LocationId { get; set; }
        public int DayOfWeek { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? BreakStart { get; set; }
        public string? BreakEnd { get; set; }
        public int MaximumAppointment { get; set; } = 20;
        public int SlotInterval { get; set; } = 15;
        public bool IsAvailable { get; set; } = true;
    }
}