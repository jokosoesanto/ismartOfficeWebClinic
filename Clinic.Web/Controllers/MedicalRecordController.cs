using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        private readonly IAppointmentDiagnosisService _diagnosisService;
        private readonly ITreatmentCatalogService _treatmentCatalogService;
        private readonly Clinic.Application.Interfaces.MasterData.IDiagnosisMasterService _diagnosisMasterService;
        private readonly IPatientService _patientService;
        private readonly Clinic.Application.Interfaces.IConditionMasterService _conditionMasterService;
        private readonly Clinic.Infrastructure.Data.AppDbContext _dbContext;
        private readonly Clinic.Application.Interfaces.Storage.IFileStorageService _storageService;

        public MedicalRecordController(
            IAppointmentTreatmentService treatmentService,
            IAppointmentService appointmentService,
            IAppointmentDiagnosisService diagnosisService,
            ITreatmentCatalogService treatmentCatalogService,
            Clinic.Application.Interfaces.MasterData.IDiagnosisMasterService diagnosisMasterService,
            IPatientService patientService,
            Clinic.Application.Interfaces.IConditionMasterService conditionMasterService,
            Clinic.Infrastructure.Data.AppDbContext dbContext,
            Clinic.Application.Interfaces.Storage.IFileStorageService storageService)
        {
            _treatmentService = treatmentService;
            _appointmentService = appointmentService;
            _diagnosisService = diagnosisService;
            _treatmentCatalogService = treatmentCatalogService;
            _diagnosisMasterService = diagnosisMasterService;
            _patientService = patientService;
            _conditionMasterService = conditionMasterService;
            _dbContext = dbContext;
            _storageService = storageService;
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
        public async Task<IActionResult> Chart(Guid patientId, [FromQuery] Guid? appointmentId = null, [FromQuery] Clinic.Domain.Enums.AppointmentStatus? historyStatus = null)
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
                
                var existingTreatments = await _treatmentService.GetTreatmentsByAppointmentIdAsync(appointmentId.Value);
                ViewBag.ExistingTreatments = existingTreatments;

                var existingDiagnoses = await _diagnosisService.GetDiagnosesByAppointmentIdAsync(appointmentId.Value);
                ViewBag.ExistingDiagnoses = existingDiagnoses;
            }

            var activeDiagnoses = (await _diagnosisMasterService.GetAllAsync())?.Where(d => d.IsActive) ?? new List<Clinic.Application.DTOs.MasterData.DiagnosisMasterDto>();
            ViewBag.DiagnosisMasters = activeDiagnoses;

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
            
            // Historical Visits
            var historicalAppointments = (await _appointmentService.GetAppointmentsByPatientIdAsync(patientId))?.ToList() ?? new List<Clinic.Application.DTOs.Operations.AppointmentDto>();
            
            if (historyStatus.HasValue)
            {
                historicalAppointments = historicalAppointments.Where(a => a.Status == historyStatus.Value).ToList();
            }

            ViewBag.HistoricalVisits = historicalAppointments;
            ViewBag.HistoryStatusFilter = historyStatus;
            
            var historicalApptIds = historicalAppointments.Select(a => a.Id).ToList();
            var historicalTreatments = await _treatmentService.GetTreatmentsByAppointmentIdsAsync(historicalApptIds);
            ViewBag.HistoricalTreatments = historicalTreatments.ToLookup(t => t.AppointmentId);

            var medications = await _dbContext.PatientMedications
                .Where(m => m.PatientId == patientId && !m.IsDeleted)
                .OrderByDescending(m => m.PrescribedDate)
                .ToListAsync();
            ViewBag.Medications = medications;

            if (appointmentId.HasValue)
            {
                var attachments = await _dbContext.FileMetadatas
                    .Where(f => f.EntityId == appointmentId.Value && !f.IsDeleted && f.Module == Clinic.Domain.Enums.StorageModule.Document)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();
                ViewBag.Attachments = attachments;
            }
            else
            {
                ViewBag.Attachments = new List<Clinic.Domain.Entities.System.FileMetadata>();
            }

            return View("Templates/MR_Chart", patient);
        }

        [HttpPost("CompleteVisit/{appointmentId}")]
        [Authorize(Policy = "Appointment.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteVisit(Guid appointmentId, [FromForm] Guid patientId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                var appt = await _appointmentService.GetByIdAsync(appointmentId);
                if (appt == null || appt.PatientId != patientId)
                {
                    TempData["ErrorMessage"] = "Invalid Appointment Context.";
                    return RedirectToAction("Chart", new { patientId });
                }

                await _appointmentService.CompleteVisitAsync(appointmentId, userId);
                TempData["SuccessMessage"] = "Visit completed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
        }

        [HttpPost("UpdateAllergyStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAllergyStatus([FromForm] Guid patientId, [FromForm] Clinic.Domain.Enums.AllergyStatus status, [FromForm] Guid? appointmentId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                await _patientService.UpdateAllergyStatusAsync(patientId, status, userId);
                TempData["SuccessMessage"] = "Allergy status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("AddAllergy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAllergy([FromForm] Guid patientId, [FromForm] string allergen, [FromForm] string? severity, [FromForm] string? notes, [FromForm] Guid? appointmentId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                await _patientService.AddAllergyAsync(patientId, allergen, severity, notes, userId);
                TempData["SuccessMessage"] = "Allergy added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("RemoveAllergy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAllergy([FromForm] Guid patientId, [FromForm] Guid allergyId, [FromForm] Guid? appointmentId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                await _patientService.RemoveAllergyAsync(patientId, allergyId, userId);
                TempData["SuccessMessage"] = "Allergy removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("UpdateMedicalHistoryStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMedicalHistoryStatus([FromForm] Guid patientId, [FromForm] Clinic.Domain.Enums.MedicalHistoryStatus status, [FromForm] Guid? appointmentId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                await _patientService.UpdateMedicalHistoryStatusAsync(patientId, status, userId);
                TempData["SuccessMessage"] = "Medical history status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("AddSystemicDisease")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSystemicDisease([FromForm] Guid patientId, [FromForm] string condition, [FromForm] string? notes, [FromForm] Guid? appointmentId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                await _patientService.AddSystemicDiseaseAsync(patientId, condition, notes, userId);
                TempData["SuccessMessage"] = "Medical condition added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("RemoveSystemicDisease")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSystemicDisease([FromForm] Guid patientId, [FromForm] Guid diseaseId, [FromForm] Guid? appointmentId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                await _patientService.RemoveSystemicDiseaseAsync(patientId, diseaseId, userId);
                TempData["SuccessMessage"] = "Medical condition removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("AddChiefComplaint")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddChiefComplaint([FromForm] Guid patientId, [FromForm] Guid appointmentId, [FromForm] string complaint, [FromForm] string? notes, [FromForm] string? toothNumber)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                var dto = new AppointmentChiefComplaintDto
                {
                    AppointmentId = appointmentId,
                    Complaint = complaint,
                    Notes = notes,
                    ToothNumber = toothNumber
                };
                await _appointmentService.AddChiefComplaintAsync(dto, userId);
                TempData["SuccessMessage"] = "Chief complaint added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("RemoveChiefComplaint")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveChiefComplaint([FromForm] Guid patientId, [FromForm] Guid appointmentId, [FromForm] Guid complaintId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                await _appointmentService.RemoveChiefComplaintAsync(appointmentId, complaintId, userId);
                TempData["SuccessMessage"] = "Chief complaint removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("SaveVitalSign")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVitalSign([FromForm] Guid patientId, [FromForm] Guid appointmentId, [FromForm] int? systolic, [FromForm] int? diastolic, [FromForm] int? heartRate, [FromForm] string temperature)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                decimal? tempValue = null;
                if (!string.IsNullOrWhiteSpace(temperature))
                {
                    var normalized = temperature.Replace(',', '.');
                    if (decimal.TryParse(normalized, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedTemp))
                    {
                        tempValue = parsedTemp;
                    }
                }

                var dto = new Clinic.Application.DTOs.Operations.AppointmentVitalSignDto
                {
                    AppointmentId = appointmentId,
                    Systolic = systolic,
                    Diastolic = diastolic,
                    HeartRate = heartRate,
                    Temperature = tempValue
                };
                await _appointmentService.SaveVitalSignAsync(dto, userId);
                TempData["SuccessMessage"] = "Vital signs saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("SaveClinicalNote")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveClinicalNote([FromForm] Guid patientId, [FromForm] Guid appointmentId, [FromForm] string? subjective, [FromForm] string? objective, [FromForm] string? assessment, [FromForm] string? plan)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid userId);
            try
            {
                var dto = new Clinic.Application.DTOs.Operations.AppointmentClinicalNoteDto
                {
                    AppointmentId = appointmentId,
                    Subjective = subjective,
                    Objective = objective,
                    Assessment = assessment,
                    Plan = plan
                };
                await _appointmentService.SaveClinicalNoteAsync(dto, userId);
                TempData["SuccessMessage"] = "Clinical note saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "notes");
        }


        [HttpGet("Create/{appointmentId}")]
        public async Task<IActionResult> Create(Guid appointmentId, [FromQuery] Guid? patientId = null, [FromQuery] string? siteNumber = null, [FromQuery] string? siteDetail = null)
        {
            var appointmentResult = await _appointmentService.GetByIdAsync(appointmentId);
            if (appointmentResult == null)
            {
                TempData["ErrorMessage"] = "Invalid Appointment.";
                return RedirectToAction("Index", "Appointment");
            }

            var catalogsResult = await _treatmentCatalogService.GetAllAsync();
            ViewBag.TreatmentItems = catalogsResult ?? new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();
            ViewBag.Treatments = ViewBag.TreatmentItems;
            
            var conditions = await _conditionMasterService.GetAllConditionsAsync();
            ViewBag.Conditions = conditions ?? new List<Clinic.Domain.Entities.MasterData.ConditionMaster>();

            var dto = new AppointmentTreatmentDto 
            { 
                AppointmentId = appointmentId,
                SiteNumber = siteNumber,
                SiteDetail = siteDetail
            };
            
            if (patientId.HasValue)
            {
                ViewBag.PatientId = patientId.Value;
            }

            var metadata = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
            ViewBag.Metadata = metadata;
            
            var existingTreatments = await _treatmentService.GetTreatmentsByAppointmentIdAsync(appointmentId);
            ViewBag.ExistingTreatments = existingTreatments;

            return View("Create", dto);
        }

        [HttpPost("Create/{appointmentId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid appointmentId, AppointmentTreatmentDto dto, [FromQuery] Guid? patientId = null)
        {
            if (appointmentId != dto.AppointmentId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var catalogsResult = await _treatmentCatalogService.GetAllAsync();
                ViewBag.TreatmentItems = catalogsResult ?? new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();
                ViewBag.Treatments = ViewBag.TreatmentItems;
                
                var conditions = await _conditionMasterService.GetAllConditionsAsync();
                ViewBag.Conditions = conditions ?? new List<Clinic.Domain.Entities.MasterData.ConditionMaster>();
                
                var metadata = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
                ViewBag.Metadata = metadata;
                
                if (patientId.HasValue)
                {
                    ViewBag.PatientId = patientId.Value;
                }
                
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
                
                if (patientId.HasValue)
                {
                    return RedirectToAction("Chart", new { patientId = patientId.Value, appointmentId = appointmentId });
                }
                
                return RedirectToAction("Create", new { appointmentId = appointmentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                
                var catalogsResult2 = await _treatmentCatalogService.GetAllAsync();
                ViewBag.TreatmentItems = catalogsResult2 != null ? catalogsResult2 : new List<Clinic.Application.DTOs.MasterData.TreatmentCatalogDto>();
                ViewBag.Treatments = ViewBag.TreatmentItems;
                
                var conditions2 = await _conditionMasterService.GetAllConditionsAsync();
                ViewBag.Conditions = conditions2 ?? new List<Clinic.Domain.Entities.MasterData.ConditionMaster>();
                
                var metadata2 = new UIMetadata { Title = "Add Treatment", ModuleName = "MedicalRecord", Mode = RenderingMode.Template };
                ViewBag.Metadata = metadata2;
                
                if (patientId.HasValue)
                {
                    ViewBag.PatientId = patientId.Value;
                }
                
                return View("Create", dto);
            }
        }

        [HttpPost("Chart/{patientId}/AddDiagnosis")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDiagnosis(Guid patientId, [FromForm] Guid appointmentId, [FromForm] Guid diagnosisMasterId, [FromForm] string? remark)
        {
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
                var dto = new Clinic.Application.DTOs.Operations.AppointmentDiagnosisDto
                {
                    AppointmentId = appointmentId,
                    DiagnosisMasterId = diagnosisMasterId,
                    Remark = remark
                };

                await _diagnosisService.CreateDiagnosisAsync(dto, userId);
                TempData["SuccessMessage"] = "Diagnosis added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to add diagnosis: {ex.Message}";
            }

            return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
        }

        [HttpPost("Chart/{patientId}/RemoveDiagnosis")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDiagnosis(Guid patientId, [FromForm] Guid appointmentId, [FromForm] Guid diagnosisId)
        {
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
                await _diagnosisService.RemoveDiagnosisAsync(appointmentId, diagnosisId, userId);
                TempData["SuccessMessage"] = "Diagnosis removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to remove diagnosis: {ex.Message}";
            }

            return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
        }

        [HttpPost("Chart/{patientId}/ApplyTreatment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyTreatmentFromChart(Guid patientId, [FromForm] Guid appointmentId, [FromForm] string treatmentName, [FromForm] string? siteNumber, [FromForm] string? siteDetail)
        {
            if (appointmentId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "A valid appointment context is required to save a treatment.";
                return RedirectToAction("Chart", new { patientId = patientId });
            }

            var appt = await _appointmentService.GetByIdAsync(appointmentId);
            if (appt == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Chart", new { patientId = patientId });
            }

            if (appt.Status == Clinic.Domain.Enums.AppointmentStatus.Completed)
            {
                TempData["ErrorMessage"] = "Treatment capture is locked because the appointment is completed.";
                return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
            }

            var catalogsResult = await _treatmentCatalogService.GetAllAsync();
            var catalog = catalogsResult?.FirstOrDefault(c => c.TreatmentName == treatmentName);
            if (catalog == null)
            {
                TempData["ErrorMessage"] = "Treatment not found in catalog.";
                return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
            }

            var dto = new AppointmentTreatmentDto
            {
                AppointmentId = appointmentId,
                TreatmentItemId = catalog.Id,
                SiteNumber = siteNumber,
                SiteDetail = siteDetail,
                ActualPrice = catalog.DefaultPrice,
                Remark = null
            };

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
                await _treatmentService.CreateTreatmentAsync(dto, userId);
                TempData["SuccessMessage"] = "Treatment saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to save treatment: {ex.Message}";
            }

            return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
        }

        [HttpPost("DeleteTreatment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTreatment(Guid treatmentId, Guid patientId, Guid appointmentId)
        {
            if (treatmentId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Invalid treatment selected for deletion.";
                return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
            }

            var result = await _treatmentService.DeleteTreatmentAsync(treatmentId);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
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

        [HttpPost("Chart/{patientId}/UpdateMedicalHistoryStatus")]
        public async Task<IActionResult> UpdateMedicalHistoryStatus(Guid patientId, [FromForm] Guid appointmentId, [FromForm] Clinic.Domain.Enums.MedicalHistoryStatus status)
        {
            await _patientService.UpdateMedicalHistoryStatusAsync(patientId, status, Guid.Empty);
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("Chart/{patientId}/AddMedication")]
        public async Task<IActionResult> AddMedication(Guid patientId, [FromForm] Guid appointmentId, [FromForm] string medicationName, [FromForm] string? dosage, [FromForm] DateTime? prescribedDate)
        {
            var med = new Clinic.Domain.Entities.MasterData.PatientMedication
            {
                PatientId = patientId,
                MedicationName = medicationName,
                Dosage = dosage,
                PrescribedDate = prescribedDate ?? DateTime.Today,
                IsActive = true
            };
            _dbContext.PatientMedications.Add(med);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("Chart/{patientId}/RemoveMedication")]
        public async Task<IActionResult> RemoveMedication(Guid patientId, [FromForm] Guid appointmentId, [FromForm] Guid medicationId)
        {
            var med = await _dbContext.PatientMedications.FindAsync(medicationId);
            if (med != null)
            {
                med.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("Chart/{patientId}/UploadAttachment")]
        public async Task<IActionResult> UploadAttachment(Guid patientId, [FromForm] Guid appointmentId, Microsoft.AspNetCore.Http.IFormFile documentUpload)
        {
            if (documentUpload != null && documentUpload.Length > 0)
            {
                using var stream = documentUpload.OpenReadStream();
                await _storageService.UploadAsync(
                    stream,
                    documentUpload.FileName,
                    documentUpload.ContentType,
                    Clinic.Domain.Enums.StorageModule.Document,
                    appointmentId,
                    Guid.Empty);
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpPost("Chart/{patientId}/RemoveAttachment")]
        public async Task<IActionResult> RemoveAttachment(Guid patientId, [FromForm] Guid appointmentId, [FromForm] Guid attachmentId)
        {
            var attachment = await _dbContext.FileMetadatas.FindAsync(attachmentId);
            if (attachment != null)
            {
                await _storageService.DeleteAsync(attachment, Guid.Empty);
            }
            return RedirectToAction("Chart", "MedicalRecord", new { patientId = patientId, appointmentId = appointmentId }, "medhistory");
        }

        [HttpGet("Attachment/{fileId}")]
        public async Task<IActionResult> DownloadAttachment(Guid fileId)
        {
            var fileMeta = await _dbContext.FileMetadatas.FindAsync(fileId);
            if (fileMeta == null || fileMeta.IsDeleted)
                return NotFound();

            var stream = await _storageService.OpenReadAsync(fileMeta);
            if (stream == null)
                return NotFound();

            return File(stream, fileMeta.MimeType ?? "application/octet-stream", fileMeta.OriginalFileName);
        }
        [HttpPost("ExecuteTreatment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecuteTreatment([FromForm] Guid patientId, [FromForm] Guid treatmentId, [FromForm] Guid appointmentId)
        {
            var result = await _treatmentService.ExecuteTreatmentAsync(treatmentId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Treatment executed successfully. It is now billable.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to execute treatment: " + result.Message;
            }
            return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
        }

        [HttpPost("RecordConsent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordConsent([FromForm] Guid patientId, [FromForm] Guid treatmentId, [FromForm] Guid appointmentId)
        {
            var userId = Guid.Empty;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedId))
                {
                    userId = parsedId;
                }
            }

            var result = await _treatmentService.RecordConsentAsync(treatmentId, userId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Consent recorded successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to record consent: " + result.Message;
            }
            return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
        }

        [HttpPost("CancelTreatment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelTreatment([FromForm] Guid patientId, [FromForm] Guid treatmentId, [FromForm] Guid appointmentId)
        {
            var result = await _treatmentService.CancelTreatmentAsync(treatmentId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Treatment cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel treatment: " + result.Message;
            }
            return RedirectToAction("Chart", new { patientId = patientId, appointmentId = appointmentId });
        }
    }
}
