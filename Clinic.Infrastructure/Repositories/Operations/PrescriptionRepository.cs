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
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly AppDbContext _context;

        public PrescriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prescription>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.Prescriptions
                .Include(p => p.PrescribedByUser)
                .Include(p => p.Items)
                .Where(p => p.AppointmentId == appointmentId && !p.IsDeleted)
                .OrderByDescending(p => p.PrescribedAt)
                .ToListAsync();
        }

        public async Task<Prescription?> GetByIdAsync(Guid id)
        {
            return await _context.Prescriptions
                .Include(p => p.PrescribedByUser)
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
        }
    }
}
