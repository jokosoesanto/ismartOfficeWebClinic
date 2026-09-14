using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic.Application.Interfaces.Operations
{
    public class PrescriptionItemDto
    {
        public string MedicationName { get; set; } = null!;
        public string Dosage { get; set; } = null!;
        public string Frequency { get; set; } = null!;
        public string Instruction { get; set; } = null!;
    }

    public interface IPrescriptionService
    {
        Task<(bool Success, string Message)> CreatePrescriptionAsync(Guid appointmentId, Guid patientId, Guid prescriberId, List<PrescriptionItemDto> items);
    }
}
