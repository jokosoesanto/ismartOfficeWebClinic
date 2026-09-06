using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces.Repositories.Operations;
using Clinic.Domain.Entities.Operations;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.Operations
{
    public class AppointmentDiagnosisRepository : IAppointmentDiagnosisRepository
    {
        private readonly AppDbContext _context;

        public AppointmentDiagnosisRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentDiagnosis?> GetByIdAsync(Guid id)
        {
            return await _context.AppointmentDiagnoses
                .Include(d => d.DiagnosisMaster)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<AppointmentDiagnosis>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.AppointmentDiagnoses
                .Include(d => d.DiagnosisMaster)
                .Where(d => d.AppointmentId == appointmentId)
                .ToListAsync();
        }

        public async Task AddAsync(AppointmentDiagnosis entity)
        {
            await _context.AppointmentDiagnoses.AddAsync(entity);
        }

        public Task UpdateAsync(AppointmentDiagnosis entity)
        {
            _context.AppointmentDiagnoses.Update(entity);
            return Task.CompletedTask;
        }
    }
}
