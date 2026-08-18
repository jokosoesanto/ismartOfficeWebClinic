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
        // Expose logic to check duplicates before saving
        Task<bool> IsDuplicateCandidateAsync(string? nationalId, string? mobile, string name, DateTime? birthDate);
    }
}
