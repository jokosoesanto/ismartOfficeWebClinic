using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.MasterData
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Include(p => p.HomeClinic)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await _context.Patients
                .Include(p => p.HomeClinic)
                .Include(p => p.PhotoFileMetadata)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Patient>> SearchAsync(
            string? mrn,
            string? nationalId,
            string? passport,
            string? name,
            string? phone,
            string? email,
            DateTime? birthDate)
        {
            var query = _context.Patients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(mrn))
                query = query.Where(p => p.MRN.Contains(mrn));

            if (!string.IsNullOrWhiteSpace(nationalId))
                query = query.Where(p => p.NationalId != null && p.NationalId.Contains(nationalId));

            if (!string.IsNullOrWhiteSpace(passport))
                query = query.Where(p => p.PassportNumber != null && p.PassportNumber.Contains(passport));

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.FullName.Contains(name));

            if (!string.IsNullOrWhiteSpace(phone))
                query = query.Where(p => (p.Mobile != null && p.Mobile.Contains(phone)) ||
                                         (p.WhatsApp != null && p.WhatsApp.Contains(phone)) ||
                                         (p.HomePhone != null && p.HomePhone.Contains(phone)));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(p => p.Email != null && p.Email.Contains(email));

            if (birthDate.HasValue)
                query = query.Where(p => p.BirthDate != null && p.BirthDate.Value.Date == birthDate.Value.Date);

            return await query
                .Include(p => p.HomeClinic)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
        }

        public void Update(Patient patient)
        {
            _context.Patients.Update(patient);
        }
    }
}
