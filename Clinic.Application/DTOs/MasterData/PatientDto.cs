using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class PatientDto : IValidatableObject
    {
        public Guid? Id { get; set; }
        
        public string? MRN { get; set; } // Readonly
        
        [Required]
        public string FullName { get; set; } = null!;
        
        public string? NationalId { get; set; }
        public string? PassportNumber { get; set; }
        
        // General Tab
        [Required(ErrorMessage = "Gender is required.")]
        public string? Gender { get; set; }
        
        [Required(ErrorMessage = "BirthDate is required.")]
        public DateTime? BirthDate { get; set; }
        public string? BloodType { get; set; }
        public string? Religion { get; set; }
        public string? Nationality { get; set; }
        public string? Language { get; set; }
        public string? Occupation { get; set; }
        public string? Education { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }

        public Guid? PhotoFileMetadataId { get; set; }

        // Contact Tab
        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? WhatsApp { get; set; }
        public string? HomePhone { get; set; }
        public string? WorkPhone { get; set; }

        // Emergency Tab
        public string? EmergencyContactName { get; set; }
        public string? EmergencyRelationship { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? EmergencyAddress { get; set; }

        // Administration Tab
        public string? PreferredCommunication { get; set; }
        public Guid? HomeClinicId { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BirthDate.HasValue && BirthDate.Value.Date >= DateTime.Now.Date)
            {
                yield return new ValidationResult("BirthDate must be earlier than today.", new[] { nameof(BirthDate) });
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                if (!Email.Contains("@") || !Email.Contains("."))
                {
                    yield return new ValidationResult("Email is invalid.", new[] { nameof(Email) });
                }
            }
        }
    }
}
