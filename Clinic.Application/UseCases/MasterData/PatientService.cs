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
    }
}
