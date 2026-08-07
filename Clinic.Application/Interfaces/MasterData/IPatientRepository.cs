using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.MasterData;

namespace Clinic.Application.Interfaces.MasterData
{
    public interface IPatientRepository
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
        Task AddAsync(Patient patient);
        void Update(Patient patient);
    }
}
