using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(Guid id);
        Task<IEnumerable<Patient>> SearchAsync(
            string? mrn,
            string? nationalId,
            string? passport,
            string? name,
            string? phone,
            string? email,
            DateTime? birthDate);
        Task<Patient> CreateAsync(Patient patient, Guid userId);
        Task<Patient> UpdateAsync(Patient patient, Guid userId);
        Task InactivateAsync(Guid id, Guid updatedBy);
        Task ReactivateAsync(Guid id, Guid updatedBy);
        
        // Allergy Operations
        Task UpdateAllergyStatusAsync(Guid patientId, Clinic.Domain.Enums.AllergyStatus status, Guid userId);
        Task<PatientAllergy> AddAllergyAsync(Guid patientId, string allergen, string? severity, string? notes, Guid userId);
        Task RemoveAllergyAsync(Guid patientId, Guid allergyId, Guid userId);

        // Medical History Operations
        Task UpdateMedicalHistoryStatusAsync(Guid patientId, Clinic.Domain.Enums.MedicalHistoryStatus status, Guid userId);
        Task<PatientMedicalHistory> AddSystemicDiseaseAsync(Guid patientId, string condition, string? notes, Guid userId);
        Task RemoveSystemicDiseaseAsync(Guid patientId, Guid diseaseId, Guid userId);

        // Insurance Operations
        Task<PatientInsurance> AddInsuranceAsync(Guid patientId, Guid insuranceId, string? memberId, string? groupId, bool isPrimary, Guid userId);
        Task<PatientInsurance> UpdateInsuranceAsync(Guid patientId, Guid patientInsuranceId, string? memberId, string? groupId, bool isPrimary, Guid userId);
        Task RemoveInsuranceAsync(Guid patientId, Guid patientInsuranceId, Guid userId);

        // Guarantor Operations
        Task<PatientGuarantor> AddGuarantorAsync(Guid patientId, PatientGuarantor guarantor, Guid userId);
        Task<PatientGuarantor> UpdateGuarantorAsync(Guid patientId, PatientGuarantor guarantor, Guid userId);
        Task RemoveGuarantorAsync(Guid patientId, Guid guarantorId, Guid userId);

        // Expose logic to check duplicates before saving
        Task<bool> IsDuplicateCandidateAsync(string? nationalId, string? mobile, string name, DateTime? birthDate);
    }
}
