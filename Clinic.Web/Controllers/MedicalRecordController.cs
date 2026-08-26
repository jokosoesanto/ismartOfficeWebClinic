using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.Operations;
using Clinic.Application.DTOs.Operations;
using System;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.MasterData;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class MedicalRecordController : Controller
    {
        private readonly IAppointmentTreatmentService _treatmentService;
        private readonly IAppointmentService _appointmentService;
        private readonly ITreatmentCatalogService _treatmentCatalogService;
        private readonly IPatientService _patientService;
        private readonly Clinic.Application.Interfaces.IConditionMasterService _conditionMasterService;

        public MedicalRecordController(
            IAppointmentTreatmentService treatmentService,
            IAppointmentService appointmentService,
            ITreatmentCatalogService treatmentCatalogService,
            IPatientService patientService,
            Clinic.Application.Interfaces.IConditionMasterService conditionMasterService)
        {
            _treatmentService = treatmentService;
            _appointmentService = appointmentService;
            _treatmentCatalogService = treatmentCatalogService;
            _patientService = patientService;
            _conditionMasterService = conditionMasterService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var patients = await _patientService.GetAllAsync();
            var metadata = new UIMetadata
            {
                Title = "Medical Records",
                ModuleName = "MedicalRecord",
                Mode = RenderingMode.Template
            };
            ViewBag.Metadata = metadata;
            return View("Templates/MR_Dashboard", patients);
        }

        [HttpGet("Chart/{patientId}")]
        public async Task<IActionResult> Chart(Guid patientId, [FromQuery] Guid? appointmentId = null)
        {
            var patient = await _patientService.GetByIdAsync(patientId);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Invalid Patient.";
                return RedirectToAction("Index");
            }

            if (appointmentId.HasValue)
            {
                var appointment = await _appointmentService.GetByIdAsync(appointmentId.Value);
                if (appointment == null || appointment.PatientId != patientId)
                {
                    TempData["ErrorMessage"] = "Invalid Appointment Context.";
                    return RedirectToAction("Index");
                }
                ViewBag.AppointmentId = appointmentId.Value;
                ViewBag.Appointment = appointment;
            }

            var metadata = new UIMetadata
            {
                Title = "Patient Chart",
                ModuleName = "MedicalRecord",
                Mode = RenderingMode.Template
            };
            ViewBag.Metadata = metadata;
            
            var treatments = await _treatmentCatalogService.GetAllAsync();
            ViewBag.Treatments = treatments ?? new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();
            
            var conditions = await _conditionMasterService.GetAllConditionsAsync();
            ViewBag.Conditions = conditions ?? new List<Clinic.Domain.Entities.MasterData.ConditionMaster>();
            
            return View("Templates/MR_Chart", patient);
        }

        [HttpGet("Create/{appointmentId}")]
        public async Task<IActionResult> Create(Guid appointmentId)
        {
            var appointmentResult = await _appointmentService.GetByIdAsync(appointmentId);
            if (appointmentResult == null)
            {
                TempData["ErrorMessage"] = "Invalid Appointment.";
                return RedirectToAction("Index", "Appointment");
            }

            var catalogsResult = await _treatmentCatalogService.GetAllAsync();
            ViewBag.TreatmentItems = catalogsResult ?? new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();

            var dto = new AppointmentTreatmentDto { AppointmentId = appointmentId };

            var metadata = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
            ViewBag.Metadata = metadata;
            
            var existingTreatments = await _treatmentService.GetTreatmentsByAppointmentIdAsync(appointmentId);
            ViewBag.ExistingTreatments = existingTreatments;

            return View("Create", dto);
        }

        [HttpPost("Create/{appointmentId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid appointmentId, AppointmentTreatmentDto dto)
        {
            if (appointmentId != dto.AppointmentId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var catalogsResult = await _treatmentCatalogService.GetAllAsync();
                ViewBag.TreatmentItems = catalogsResult ?? new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();
                
                var metadata = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
                ViewBag.Metadata = metadata;
                return View("Create", dto);
            }

            // Using empty guid for user as a placeholder, or fetch from claims
            var userId = Guid.Empty;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedId))
                {
                    userId = parsedId;
                }
            }

            try
            {
                var result = await _treatmentService.CreateTreatmentAsync(dto, userId);
                TempData["SuccessMessage"] = "Treatment created successfully.";
                return RedirectToAction("Create", new { appointmentId = appointmentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                
                var catalogsResult2 = await _treatmentCatalogService.GetAllAsync();
                ViewBag.TreatmentItems = catalogsResult2 != null ? catalogsResult2 : new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();
                
                var metadata2 = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
                ViewBag.Metadata = metadata2;
                return View("Create", dto);
            }
        }

        [HttpGet("Chart/{patientId}/Treatment/Add")]
        public async Task<IActionResult> AddTreatment(Guid patientId, [FromQuery] Guid appointmentId, [FromQuery] string? siteNumber = null, [FromQuery] string? siteDetail = null)
        {
            var appointmentResult = await _appointmentService.GetByIdAsync(appointmentId);
            if (appointmentResult == null || appointmentResult.PatientId != patientId)
            {
                TempData["ErrorMessage"] = "Invalid Appointment Context.";
                return RedirectToAction("Chart", new { patientId = patientId });
            }

            var catalogsResult = await _treatmentCatalogService.GetAllAsync();
            ViewBag.TreatmentItems = catalogsResult ?? new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();

            var dto = new AppointmentTreatmentDto 
            { 
                AppointmentId = appointmentId,
                SiteNumber = siteNumber,
                SiteDetail = siteDetail
            };

            var metadata = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
            ViewBag.Metadata = metadata;
            
            var existingTreatments = await _treatmentService.GetTreatmentsByAppointmentIdAsync(appointmentId);
            ViewBag.ExistingTreatments = existingTreatments;
            ViewBag.PatientId = patientId;

            return View("AddTreatment", dto);
        }

        [HttpPost("Chart/{patientId}/Treatment/Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTreatment(Guid patientId, [FromQuery] Guid appointmentId, AppointmentTreatmentDto dto)
        {
            if (appointmentId != dto.AppointmentId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var catalogsResult = await _treatmentCatalogService.GetAllAsync();
                ViewBag.TreatmentItems = catalogsResult ?? new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();
                
                var metadata = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
                ViewBag.Metadata = metadata;
                ViewBag.PatientId = patientId;
                return View("AddTreatment", dto);
            }

            var userId = Guid.Empty;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedId))
                {
                    userId = parsedId;
                }
            }

            try
            {
                var result = await _treatmentService.CreateTreatmentAsync(dto, userId);
                TempData["SuccessMessage"] = "Treatment created successfully.";
                return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                
                var catalogsResult2 = await _treatmentCatalogService.GetAllAsync();
                ViewBag.TreatmentItems = catalogsResult2 != null ? catalogsResult2 : new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();
                
                var metadata2 = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
                ViewBag.Metadata = metadata2;
                ViewBag.PatientId = patientId;
                return View("AddTreatment", dto);
            }
        }

        [HttpGet("History/{id}")]
        public IActionResult History(string id)
        {
            var metadata = new UIMetadata { Title = "Treatment History", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
            return View("Templates/MR_History", metadata);
        }

        [HttpGet("{id}")]
        public IActionResult Details(string id)
        {
            var metadata = new UIMetadata { Title = "Treatment Detail", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
            return View("Templates/MR_Detail", metadata);
        }
    }
}
