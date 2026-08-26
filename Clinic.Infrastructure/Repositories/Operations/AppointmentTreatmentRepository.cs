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
    public class AppointmentTreatmentRepository : IAppointmentTreatmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentTreatmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentTreatment?> GetByIdAsync(Guid id)
        {
            return await _context.AppointmentTreatments
                .Include(t => t.TreatmentItem)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<AppointmentTreatment>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.AppointmentTreatments
                .Include(t => t.TreatmentItem)
                .Where(t => t.AppointmentId == appointmentId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(AppointmentTreatment treatment)
        {
            await _context.AppointmentTreatments.AddAsync(treatment);
        }
    }
}
