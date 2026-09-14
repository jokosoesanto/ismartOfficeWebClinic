using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Repositories.Operations
{
    public interface IPrescriptionRepository
    {
        Task<IEnumerable<Prescription>> GetByAppointmentIdAsync(Guid appointmentId);
        Task<Prescription?> GetByIdAsync(Guid id);
        Task AddAsync(Prescription prescription);
    }
}
