using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.UseCases.MasterData
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INumberSequenceService _sequenceService;

        public PatientService(
            IPatientRepository repository, 
            IUnitOfWork unitOfWork,
            INumberSequenceService sequenceService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _sequenceService = sequenceService;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string? mrn, string? nationalId, string? passport, string? name, string? phone, string? email, DateTime? birthDate)
        {
            return await _repository.SearchAsync(mrn, nationalId, passport, name, phone, email, birthDate);
        }

        public async Task<Patient> CreateAsync(Patient patient, Guid userId)
        {
            // 1. Generate Immutable MRN
            patient.MRN = await _sequenceService.GenerateSequenceAsync("MR");
            
            // 2. Default preferred communication if null
            if (string.IsNullOrWhiteSpace(patient.PreferredCommunication))
            {
                patient.PreferredCommunication = "Phone";
            }
            
            patient.CreatedAt = DateTime.UtcNow;
            patient.CreatedBy = userId;
            patient.IsDeleted = false;
            patient.Status = "Active";

            await _repository.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();
            
            return patient;
        }

        public async Task<Patient> UpdateAsync(Patient patient, Guid userId)
        {
            // ENTERPRISE AGGREGATE UPDATE STANDARD:
            // The entity 'patient' is assumed to be tracked from the Controller's GetByIdAsync call.
            // Do not perform a redundant GetByIdAsync() here which could trigger dual-load issues.
            
            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = userId;
            
            _repository.Update(patient);
            await _unitOfWork.SaveChangesAsync();
            return patient;
        }

        public async Task InactivateAsync(Guid id, Guid updatedBy)
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient != null)
            {
                patient.Status = "Inactive";
                patient.UpdatedAt = DateTime.UtcNow;
                patient.UpdatedBy = updatedBy;
                
                _repository.Update(patient);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task ReactivateAsync(Guid id, Guid updatedBy)
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient != null)
            {
                patient.Status = "Active";
                patient.UpdatedAt = DateTime.UtcNow;
                patient.UpdatedBy = updatedBy;
                
                _repository.Update(patient);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> IsDuplicateCandidateAsync(string? nationalId, string? mobile, string name, DateTime? birthDate)
        {
            // Minimal duplicate checks
            if (!string.IsNullOrWhiteSpace(nationalId))
            {
                var matches = await _repository.SearchAsync(null, nationalId, null, null, null, null, null);
                if (matches.Any()) return true;
            }

            if (!string.IsNullOrWhiteSpace(mobile))
            {
                var matches = await _repository.SearchAsync(null, null, null, null, mobile, null, null);
                if (matches.Any()) return true;
            }

            if (!string.IsNullOrWhiteSpace(name) && birthDate.HasValue)
            {
                var matches = await _repository.SearchAsync(null, null, null, name, null, null, birthDate);
                if (matches.Any()) return true;
            }

            return false;
        }
        public async Task UpdateAllergyStatusAsync(Guid patientId, Clinic.Domain.Enums.AllergyStatus status, Guid userId)
        {
            var patient = await _repository.GetByIdAsync(patientId);
            if (patient == null) throw new Exception("Patient not found.");

            if (status == Clinic.Domain.Enums.AllergyStatus.NoKnownAllergy)
            {
                // Soft delete existing allergies
                if (patient.Allergies != null)
                {
                    foreach (var allergy in patient.Allergies.Where(a => !a.IsDeleted))
                    {
                        allergy.IsDeleted = true;
                        allergy.DeletedAt = DateTime.UtcNow;
                        allergy.DeletedBy = userId;
                    }
                }
            }
            else if (status == Clinic.Domain.Enums.AllergyStatus.HasAllergies)
            {
                if (patient.Allergies == null || !patient.Allergies.Any(a => !a.IsDeleted))
                {
                    throw new Exception("Cannot set status to HasAllergies without active allergy records.");
                }
            }

            patient.AllergyStatus = status;
            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = userId;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PatientAllergy> AddAllergyAsync(Guid patientId, string allergen, string? severity, string? notes, Guid userId)
        {
            var patient = await _repository.GetByIdAsync(patientId);
            if (patient == null) throw new Exception("Patient not found.");

            // FORENSIC FIX: Explicitly set Id to Guid.Empty so EF Core ChangeTracker 
            // natively recognizes this as a new entity (IsKeySet == false) during graph traversal,
            // correctly marking it as 'Added' rather than 'Modified', and generating an INSERT.
            var allergy = new PatientAllergy
            {
                Id = Guid.Empty, 
                PatientId = patientId,
                Allergen = allergen,
                Severity = severity,
                Notes = notes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            patient.Allergies.Add(allergy);
            patient.AllergyStatus = Clinic.Domain.Enums.AllergyStatus.HasAllergies;
            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = userId;

            await _unitOfWork.SaveChangesAsync();

            return allergy;
        }

        public async Task RemoveAllergyAsync(Guid patientId, Guid allergyId, Guid userId)
        {
            var patient = await _repository.GetByIdAsync(patientId);
            if (patient == null) throw new Exception("Patient not found.");

            var allergy = patient.Allergies.FirstOrDefault(a => a.Id == allergyId && !a.IsDeleted);
            if (allergy == null) throw new Exception("Allergy not found.");

            var activeAllergies = patient.Allergies.Count(a => !a.IsDeleted);
            if (activeAllergies <= 1 && patient.AllergyStatus == Clinic.Domain.Enums.AllergyStatus.HasAllergies)
            {
                throw new Exception("Cannot remove the last allergy while status is 'Has Allergies'. Explicitly set status to 'No Known Allergy' or 'Not Recorded' instead.");
            }

            allergy.IsDeleted = true;
            allergy.DeletedAt = DateTime.UtcNow;
            allergy.DeletedBy = userId;

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = userId;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMedicalHistoryStatusAsync(Guid patientId, Clinic.Domain.Enums.MedicalHistoryStatus status, Guid userId)
        {
            var patient = await _repository.GetByIdAsync(patientId);
            if (patient == null) throw new Exception("Patient not found.");

            if (status == Clinic.Domain.Enums.MedicalHistoryStatus.NoKnownMedicalCondition)
            {
                // Soft delete existing active diseases
                if (patient.SystemicDiseases != null)
                {
                    foreach (var disease in patient.SystemicDiseases.Where(d => !d.IsDeleted && d.IsActive))
                    {
                        disease.IsDeleted = true;
                        disease.DeletedAt = DateTime.UtcNow;
                        disease.DeletedBy = userId;
                    }
                }
            }
            else if (status == Clinic.Domain.Enums.MedicalHistoryStatus.HasMedicalConditions)
            {
                if (patient.SystemicDiseases == null || !patient.SystemicDiseases.Any(d => !d.IsDeleted))
                {
                    throw new Exception("Cannot set status to HasMedicalConditions without active condition records.");
                }
            }

            patient.MedicalHistoryStatus = status;
            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = userId;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PatientMedicalHistory> AddSystemicDiseaseAsync(Guid patientId, string condition, string? notes, Guid userId)
        {
            var patient = await _repository.GetByIdAsync(patientId);
            if (patient == null) throw new Exception("Patient not found.");

            // Duplicate prevention for active conditions
            if (patient.SystemicDiseases.Any(d => !d.IsDeleted && d.IsActive && d.Condition.Equals(condition, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception($"Condition '{condition}' is already actively recorded for this patient.");
            }

            var disease = new PatientMedicalHistory
            {
                Id = Guid.Empty, // EF Core Insert Pattern
                PatientId = patientId,
                Condition = condition,
                Notes = notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            patient.SystemicDiseases.Add(disease);
            patient.MedicalHistoryStatus = Clinic.Domain.Enums.MedicalHistoryStatus.HasMedicalConditions;
            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = userId;

            await _unitOfWork.SaveChangesAsync();

            return disease;
        }

        public async Task RemoveSystemicDiseaseAsync(Guid patientId, Guid diseaseId, Guid userId)
        {
            var patient = await _repository.GetByIdAsync(patientId);
            if (patient == null) throw new Exception("Patient not found.");

            var disease = patient.SystemicDiseases.FirstOrDefault(d => d.Id == diseaseId && !d.IsDeleted);
            if (disease == null) throw new Exception("Condition not found.");

            var activeConditions = patient.SystemicDiseases.Count(d => !d.IsDeleted);
            if (activeConditions <= 1 && patient.MedicalHistoryStatus == Clinic.Domain.Enums.MedicalHistoryStatus.HasMedicalConditions)
            {
                throw new Exception("Cannot remove the last condition while status is 'Has Medical Conditions'. Explicitly set status to 'No Known Medical Condition' or 'Not Recorded' instead.");
            }

            disease.IsDeleted = true;
            disease.DeletedAt = DateTime.UtcNow;
            disease.DeletedBy = userId;

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = userId;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
