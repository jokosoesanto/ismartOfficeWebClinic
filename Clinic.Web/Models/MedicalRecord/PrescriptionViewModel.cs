using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Web.Models.MedicalRecord
{
    public class PrescriptionViewModel
    {
        [Required]
        public Guid AppointmentId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        public List<PrescriptionItemViewModel> Items { get; set; } = new List<PrescriptionItemViewModel>();
    }

    public class PrescriptionItemViewModel
    {
        [Required(ErrorMessage = "Medication name is required")]
        public string MedicationName { get; set; } = null!;

        [Required(ErrorMessage = "Dosage is required")]
        public string Dosage { get; set; } = null!;

        [Required(ErrorMessage = "Frequency is required")]
        public string Frequency { get; set; } = null!;

        [Required(ErrorMessage = "Instruction is required")]
        public string Instruction { get; set; } = null!;
    }
}
