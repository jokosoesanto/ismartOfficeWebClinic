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


        // Expose logic to check duplicates before saving
        Task<bool> IsDuplicateCandidateAsync(string? nationalId, string? mobile, string name, DateTime? birthDate);
    }
}
