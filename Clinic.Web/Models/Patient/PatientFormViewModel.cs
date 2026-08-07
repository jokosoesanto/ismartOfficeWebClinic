using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Clinic.Application.DTOs.MasterData;

namespace Clinic.Web.Models.Patient
{
    public class PatientFormViewModel
    {
        public PatientDto Patient { get; set; } = new PatientDto();
        
        public IEnumerable<SelectListItem> Locations { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Genders { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> BloodTypes { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Religions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Nationalities { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Languages { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Occupations { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Educations { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> MaritalStatuses { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Countries { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Provinces { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Relationships { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Statuses { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> PreferredCommunications { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
