using System;
using Clinic.Domain.Entities.System;

namespace Clinic.Domain.Entities.MasterData
{
    public class Patient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string MRN { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? NationalId { get; set; }
        public string? PassportNumber { get; set; }

        public string? Gender { get; set; } // Reference Category: Gender
        public DateTime? BirthDate { get; set; }
        public string? BloodType { get; set; } // Reference Category: BloodType
        public string? Religion { get; set; } // Reference Category: Religion
        public string? Nationality { get; set; } // Reference Category: Nationality
        public string? Language { get; set; } // Reference Category: Language
        public string? Occupation { get; set; } // Reference Category: Occupation
        public string? Education { get; set; } // Reference Category: Education
        public string? MaritalStatus { get; set; } // Reference Category: MaritalStatus
        public string? Category { get; set; } // Reference Category: PatientCategory
        public string? Status { get; set; } // Reference Category: PatientStatus

        public Guid? PhotoFileMetadataId { get; set; }
        public FileMetadata? PhotoFileMetadata { get; set; }

        // Contact fields
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; } // Reference Category: Country
        public string? PostalCode { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? WhatsApp { get; set; }
        public string? HomePhone { get; set; }
        public string? WorkPhone { get; set; }

        // Emergency fields
        public string? EmergencyContactName { get; set; }
        public string? EmergencyRelationship { get; set; } // Reference Category: Relationship
        public string? EmergencyPhone { get; set; }
        public string? EmergencyAddress { get; set; }

        // Communication
        public string? PreferredCommunication { get; set; } // Reference Category: PreferredCommunication (Default: Phone, WhatsApp, SMS, Email)

        // Administration
        public Guid? HomeClinicId { get; set; }
        public Location? HomeClinic { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? Notes { get; set; }

        // Audit & Soft Delete
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
