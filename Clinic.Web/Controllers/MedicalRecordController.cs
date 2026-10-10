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
        private readonly IPrescriptionService _prescriptionService;
        private readonly Clinic.Application.Interfaces.Repositories.Operations.IPrescriptionRepository _prescriptionRepository;

        public MedicalRecordController(
            IAppointmentTreatmentService treatmentService,
            IAppointmentService appointmentService,
            IAppointmentDiagnosisService diagnosisService,
            ITreatmentCatalogService treatmentCatalogService,
            Clinic.Application.Interfaces.MasterData.IDiagnosisMasterService diagnosisMasterService,
            IPatientService patientService,
            Clinic.Application.Interfaces.IConditionMasterService conditionMasterService,
            Clinic.Infrastructure.Data.AppDbContext dbContext,
            Clinic.Application.Interfaces.Storage.IFileStorageService storageService,
            IPrescriptionService prescriptionService,
            Clinic.Application.Interfaces.Repositories.Operations.IPrescriptionRepository prescriptionRepository)
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
            _prescriptionService = prescriptionService;
            _prescriptionRepository = prescriptionRepository;
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

                var prescriptions = await _prescriptionRepository.GetByAppointmentIdAsync(appointmentId.Value);
                ViewBag.Prescriptions = prescriptions;

                var postTreatmentInstruction = await _dbContext.PostTreatmentInstructions
                    .FirstOrDefaultAsync(x => x.PatientId == patientId && x.AppointmentId == appointmentId);
                ViewBag.PostTreatmentInstruction = postTreatmentInstruction;

                var appointmentFollowUp = await _dbContext.AppointmentFollowUps
                    .FirstOrDefaultAsync(x => x.PatientId == patientId && x.AppointmentId == appointmentId);
                ViewBag.AppointmentFollowUp = appointmentFollowUp;
            }
            else
            {
                ViewBag.Attachments = new List<Clinic.Domain.Entities.System.FileMetadata>();
                ViewBag.Prescriptions = new List<Clinic.Domain.Entities.Operations.Prescription>();
            }

            var timeline = new List<Clinic.Application.DTOs.Operations.PatientTimelineItemDto>();

            var timelineAppointments = await _dbContext.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.AppointmentType)
                .Where(a => a.PatientId == patientId && !a.IsDeleted)
                .ToListAsync();

            foreach(var a in timelineAppointments)
            {
                timeline.Add(new Clinic.Application.DTOs.Operations.PatientTimelineItemDto
                {
                    EventDate = a.Date.Add(a.StartTime),
                    EventType = "Appointment",
                    Title = $"Appointment with {(a.Doctor?.Title != null ? a.Doctor.Title + " " : "Dr ")}{a.Doctor?.FullName}",
                    Summary = $"Status: {a.Status}, Type: {a.AppointmentType?.Name}",
                    ReferenceId = a.Id,
                    Icon = "bi-calendar-check"
                });
            }

            var chiefComplaints = await _dbContext.Set<Clinic.Domain.Entities.Operations.AppointmentChiefComplaint>()
                .Include(c => c.Appointment)
                .Where(c => c.Appointment!.PatientId == patientId && !c.IsDeleted)
                .ToListAsync();

            foreach(var c in chiefComplaints)
            {
                timeline.Add(new Clinic.Application.DTOs.Operations.PatientTimelineItemDto
                {
                    EventDate = c.CreatedAt,
                    EventType = "Chief Complaint",
                    Title = "Chief Complaint",
                    Summary = c.Complaint,
                    ReferenceId = c.Id,
                    Icon = "bi-person-exclamation"
                });
            }

            var diagnosesTimeline = await _dbContext.AppointmentDiagnoses
                .Include(d => d.Appointment)
                .Include(d => d.DiagnosisMaster)
                .Where(d => d.Appointment!.PatientId == patientId && !d.IsDeleted)
                .ToListAsync();

            foreach(var d in diagnosesTimeline)
            {
                timeline.Add(new Clinic.Application.DTOs.Operations.PatientTimelineItemDto
                {
                    EventDate = d.CreatedAt,
                    EventType = "Diagnosis",
                    Title = d.DiagnosisMaster?.DiagnosisCode + " - " + d.DiagnosisMaster?.DiagnosisName,
                    Summary = d.Remark ?? "No additional notes",
                    ReferenceId = d.Id,
                    Icon = "bi-clipboard2-pulse"
                });
            }

            var treatmentsTimeline = await _dbContext.AppointmentTreatments
                .Include(t => t.Appointment)
                .Include(t => t.TreatmentItem)
                .Where(t => t.Appointment!.PatientId == patientId && !t.Appointment.IsDeleted)
                .ToListAsync();

            foreach(var t in treatmentsTimeline)
            {
                timeline.Add(new Clinic.Application.DTOs.Operations.PatientTimelineItemDto
                {
                    EventDate = t.CreatedAt,
                    EventType = "Treatment",
                    Title = t.TreatmentItem?.TreatmentName ?? "Treatment",
                    Summary = $"Status: {t.Status}",
                    ReferenceId = t.Id,
                    Icon = "bi-bandaid"
                });
            }
            
            var clinicalNotes = await _dbContext.AppointmentClinicalNotes
                .Include(n => n.Appointment)
                .Where(n => n.Appointment!.PatientId == patientId && !n.IsDeleted)
                .ToListAsync();

            foreach(var n in clinicalNotes)
            {
                timeline.Add(new Clinic.Application.DTOs.Operations.PatientTimelineItemDto
                {
                    EventDate = n.CreatedAt,
                    EventType = "Clinical Note",
                    Title = "Clinical Note",
                    Summary = $"Subj: {n.Subjective?.Substring(0, Math.Min(n.Subjective.Length, 30))}...",
                    ReferenceId = n.Id,
                    Icon = "bi-journal-text"
                });
            }

            var prescriptionsTimeline = await _dbContext.Prescriptions
                .Include(p => p.Appointment)
                .Where(p => p.PatientId == patientId && !p.IsDeleted)
                .ToListAsync();

            foreach(var p in prescriptionsTimeline)
            {
                timeline.Add(new Clinic.Application.DTOs.Operations.PatientTimelineItemDto
                {
                    EventDate = p.PrescribedAt,
                    EventType = "Prescription",
                    Title = "Prescription Issued",
                    Summary = $"Status: {p.Status}",
                    ReferenceId = p.Id,
                    Icon = "bi-capsule"
                });
            }

            var paymentsTimeline = await _dbContext.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Appointment)
                .Where(p => p.Invoice!.Appointment!.PatientId == patientId && !p.IsDeleted)
                .ToListAsync();

            foreach(var p in paymentsTimeline)
            {
                timeline.Add(new Clinic.Application.DTOs.Operations.PatientTimelineItemDto
                {
                    EventDate = p.PaymentDate,
                    EventType = "Payment",
                    Title = $"Payment Received",
                    Summary = $"Amount: {p.Amount:C} via {p.PaymentMethod}",
                    ReferenceId = p.Id,
                    Icon = "bi-cash-coin"
                });
            }

            ViewBag.Timeline = timeline.OrderByDescending(x => x.EventDate).ToList();

            // B1 - Patient Financial Ledger MVP
            var ledgerInvoices = await _dbContext.Invoices
                .AsNoTracking()
                .Include(i => i.Appointment)
                .Where(i => i.Appointment!.PatientId == patientId && !i.IsDeleted)
                .ToListAsync();

            var ledgerPayments = await _dbContext.Payments
                .AsNoTracking()
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Appointment)
                .Include(p => p.Refunds)
                .Where(p => p.Invoice!.Appointment!.PatientId == patientId && !p.IsDeleted)
                .ToListAsync();

            var ledgerAdjustments = await _dbContext.InvoiceAdjustments
                .AsNoTracking()
                .Include(a => a.Invoice)
                .ThenInclude(i => i!.Appointment)
                .Where(a => a.Invoice!.Appointment!.PatientId == patientId && !a.IsDeleted)
                .ToListAsync();

            var patientCredits = await _dbContext.PatientCredits
                .AsNoTracking()
                .Where(c => c.PatientId == patientId && !c.IsDeleted)
                .ToListAsync();

            var ledger = new PatientLedgerDto
            {
                PatientId = patientId,
                PatientName = patient.FullName
            };

            foreach (var inv in ledgerInvoices)
            {
                ledger.Transactions.Add(new PatientLedgerTransactionDto
                {
                    TransactionDate = inv.InvoiceDate,
                    TransactionType = "Invoice",
                    ReferenceNumber = inv.InvoiceNumber,
                    Description = "Charges for visit",
                    Debit = inv.TotalAmount,
                    Credit = 0,
                    SortOrder = 1,
                    SourceId = inv.Id
                });
                ledger.TotalDebit += inv.TotalAmount;
            }

            foreach (var pay in ledgerPayments)
            {
                ledger.Transactions.Add(new PatientLedgerTransactionDto
                {
                    TransactionDate = pay.PaymentDate,
                    TransactionType = "Payment",
                    ReferenceNumber = pay.ReceiptNumber,
                    Description = pay.PaymentMethod + (string.IsNullOrEmpty(pay.ReferenceNumber) ? "" : $" ({pay.ReferenceNumber})"),
                    Debit = 0,
                    Credit = pay.Amount,
                    SortOrder = 2,
                    SourceId = pay.Id
                });
                ledger.TotalCredit += pay.Amount;
                
                if (pay.Refunds != null)
                {
                    foreach (var refund in pay.Refunds.Where(r => !r.IsDeleted))
                    {
                        ledger.Transactions.Add(new PatientLedgerTransactionDto
                        {
                            TransactionDate = refund.RefundDate,
                            TransactionType = "Refund",
                            ReferenceNumber = $"REF-{refund.Id.ToString().Substring(0, 8).ToUpper()}",
                            Description = "Payment Refund" + (string.IsNullOrEmpty(refund.Reason) ? "" : $" ({refund.Reason})"),
                            Debit = refund.Amount,
                            Credit = 0,
                            SortOrder = 4, // After adjustment
                            SourceId = refund.Id
                        });
                        ledger.TotalDebit += refund.Amount;
                    }
                }
            }

            foreach (var adj in ledgerAdjustments)
            {
                ledger.Transactions.Add(new PatientLedgerTransactionDto
                {
                    TransactionDate = adj.AdjustmentDate,
                    TransactionType = "Adjustment",
                    ReferenceNumber = $"ADJ-{adj.Id.ToString().Substring(0,8).ToUpper()}",
                    Description = adj.AdjustmentType + (string.IsNullOrEmpty(adj.Reason) ? "" : $" ({adj.Reason})"),
                    Debit = 0,
                    Credit = adj.Amount,
                    SortOrder = 3,
                    SourceId = adj.Id
                });
                ledger.TotalCredit += adj.Amount;
            }

            foreach (var cred in patientCredits)
            {
                ledger.Transactions.Add(new PatientLedgerTransactionDto
                {
                    TransactionDate = cred.CreditDate,
                    TransactionType = "Patient Credit",
                    ReferenceNumber = $"CRE-{cred.Id.ToString().Substring(0,8).ToUpper()}",
                    Description = "Prepayment / Patient Credit" + (string.IsNullOrEmpty(cred.Reason) ? "" : $" ({cred.Reason})"),
                    Debit = 0,
                    Credit = cred.Amount,
                    SortOrder = 1,
                    SourceId = cred.Id
                });
                ledger.TotalCredit += cred.Amount;
            }

            ViewBag.AvailableCredit = patientCredits.Sum(c => c.Amount);
            ViewBag.PatientCredits = patientCredits;

            // Order by date ASC to calculate running balance mathematically
            ledger.Transactions = ledger.Transactions
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.SortOrder)
                .ThenBy(t => t.SourceId)
                .ToList();

            decimal runningBal = 0;
            foreach (var t in ledger.Transactions)
            {
                runningBal = runningBal + t.Debit - t.Credit;
                t.RunningBalance = runningBal;
            }

            // Reverse for UI display (Newest -> Oldest)
            ledger.Transactions.Reverse();

            ViewBag.PatientLedger = ledger;

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
        [HttpPost("CreatePrescription")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrescription([FromForm] Clinic.Web.Models.MedicalRecord.PrescriptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed for prescription.";
                return RedirectToAction("Chart", new { patientId = model.PatientId, appointmentId = model.AppointmentId });
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var prescriberId))
            {
                TempData["ErrorMessage"] = "Failed to identify prescriber.";
                return RedirectToAction("Chart", new { patientId = model.PatientId, appointmentId = model.AppointmentId });
            }

            var dtoItems = model.Items.Select(i => new Clinic.Application.Interfaces.Operations.PrescriptionItemDto
            {
                MedicationName = i.MedicationName,
                Dosage = i.Dosage,
                Frequency = i.Frequency,
                Instruction = i.Instruction
            }).ToList();

            var result = await _prescriptionService.CreatePrescriptionAsync(model.AppointmentId, model.PatientId, prescriberId, dtoItems);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Chart", new { patientId = model.PatientId, appointmentId = model.AppointmentId });
        }

        [HttpPost("SavePostTreatmentInstruction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePostTreatmentInstruction([FromForm] Guid patientId, [FromForm] Guid appointmentId, [FromForm] string instructionText, [FromServices] IPostTreatmentInstructionService _instructionService)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out var userId);

            try
            {
                await _instructionService.AddInstructionAsync(appointmentId, patientId, userId, instructionText);
                TempData["SuccessMessage"] = "Post-Treatment Instructions saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Chart", new { id = patientId, activeTab = "clinicalnotes", appointmentId = appointmentId });
        }

        [HttpPost("SaveFollowUp")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFollowUp([FromForm] Guid patientId, [FromForm] Guid appointmentId, [FromForm] bool isFollowUpRequired, [FromForm] DateTime? recommendedDate, [FromForm] string reason)
        {
            if (isFollowUpRequired && !recommendedDate.HasValue)
            {
                TempData["ErrorMessage"] = "Recommended Date/Time is required when Follow-up is required.";
                return RedirectToAction("Chart", new { id = patientId, activeTab = "clinicalnotes", appointmentId = appointmentId });
            }

            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out var userId);

            try
            {
                var existingFollowUp = await _dbContext.AppointmentFollowUps
                    .FirstOrDefaultAsync(x => x.PatientId == patientId && x.AppointmentId == appointmentId);

                if (existingFollowUp == null)
                {
                    existingFollowUp = new Clinic.Domain.Entities.Operations.AppointmentFollowUp
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientId,
                        AppointmentId = appointmentId,
                        IsFollowUpRequired = isFollowUpRequired,
                        RecommendedDate = isFollowUpRequired ? recommendedDate : null,
                        Reason = reason,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _dbContext.AppointmentFollowUps.Add(existingFollowUp);
                }
                else
                {
                    existingFollowUp.IsFollowUpRequired = isFollowUpRequired;
                    existingFollowUp.RecommendedDate = isFollowUpRequired ? recommendedDate : null;
                    existingFollowUp.Reason = reason;
                    existingFollowUp.UpdatedBy = userId;
                    existingFollowUp.UpdatedAt = DateTime.UtcNow;
                    _dbContext.AppointmentFollowUps.Update(existingFollowUp);
                }

                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Follow-up / Next Control saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to save Follow-up: " + ex.Message;
            }

            return RedirectToAction("Chart", new { id = patientId, activeTab = "clinicalnotes", appointmentId = appointmentId });
        }
        [HttpGet("Chart/{id}/Statement")]
        public async Task<IActionResult> Statement(Guid id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var patient = await _dbContext.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (patient == null) return NotFound("Patient not found");

            // Default to current month if not provided
            var sDate = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var eDate = endDate ?? sDate.AddMonths(1).AddDays(-1);

            // B1 - Patient Financial Ledger MVP
            var ledgerInvoices = await _dbContext.Invoices
                .AsNoTracking()
                .Include(i => i.Appointment)
                .Where(i => i.Appointment!.PatientId == id && !i.IsDeleted)
                .ToListAsync();

            var ledgerPayments = await _dbContext.Payments
                .AsNoTracking()
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Appointment)
                .Include(p => p.Refunds)
                .Where(p => p.Invoice!.Appointment!.PatientId == id && !p.IsDeleted)
                .ToListAsync();

            var ledgerAdjustments = await _dbContext.InvoiceAdjustments
                .AsNoTracking()
                .Include(a => a.Invoice)
                .ThenInclude(i => i!.Appointment)
                .Where(a => a.Invoice!.Appointment!.PatientId == id && !a.IsDeleted)
                .ToListAsync();

            var patientCredits = await _dbContext.PatientCredits
                .AsNoTracking()
                .Where(c => c.PatientId == id && !c.IsDeleted)
                .ToListAsync();

            var allTransactions = new List<Clinic.Application.DTOs.Operations.PatientLedgerTransactionDto>();

            foreach (var inv in ledgerInvoices)
            {
                allTransactions.Add(new Clinic.Application.DTOs.Operations.PatientLedgerTransactionDto
                {
                    TransactionDate = inv.InvoiceDate,
                    TransactionType = "Invoice",
                    ReferenceNumber = inv.InvoiceNumber,
                    Description = "Charges for visit",
                    Debit = inv.TotalAmount,
                    Credit = 0,
                    SortOrder = 1,
                    SourceId = inv.Id
                });
            }

            foreach (var pay in ledgerPayments)
            {
                allTransactions.Add(new Clinic.Application.DTOs.Operations.PatientLedgerTransactionDto
                {
                    TransactionDate = pay.PaymentDate,
                    TransactionType = "Payment",
                    ReferenceNumber = pay.ReceiptNumber,
                    Description = pay.PaymentMethod + (string.IsNullOrEmpty(pay.ReferenceNumber) ? "" : $" ({pay.ReferenceNumber})"),
                    Debit = 0,
                    Credit = pay.Amount,
                    SortOrder = 2,
                    SourceId = pay.Id
                });
                
                if (pay.Refunds != null)
                {
                    foreach (var refund in pay.Refunds.Where(r => !r.IsDeleted))
                    {
                        allTransactions.Add(new Clinic.Application.DTOs.Operations.PatientLedgerTransactionDto
                        {
                            TransactionDate = refund.RefundDate,
                            TransactionType = "Refund",
                            ReferenceNumber = $"REF-{refund.Id.ToString().Substring(0, 8).ToUpper()}",
                            Description = "Payment Refund" + (string.IsNullOrEmpty(refund.Reason) ? "" : $" ({refund.Reason})"),
                            Debit = refund.Amount,
                            Credit = 0,
                            SortOrder = 4,
                            SourceId = refund.Id
                        });
                    }
                }
            }

            foreach (var adj in ledgerAdjustments)
            {
                allTransactions.Add(new Clinic.Application.DTOs.Operations.PatientLedgerTransactionDto
                {
                    TransactionDate = adj.AdjustmentDate,
                    TransactionType = "Adjustment",
                    ReferenceNumber = $"ADJ-{adj.Id.ToString().Substring(0,8).ToUpper()}",
                    Description = adj.AdjustmentType + (string.IsNullOrEmpty(adj.Reason) ? "" : $" ({adj.Reason})"),
                    Debit = 0,
                    Credit = adj.Amount,
                    SortOrder = 3,
                    SourceId = adj.Id
                });
            }

            foreach (var cred in patientCredits)
            {
                allTransactions.Add(new Clinic.Application.DTOs.Operations.PatientLedgerTransactionDto
                {
                    TransactionDate = cred.CreditDate,
                    TransactionType = "Patient Credit",
                    ReferenceNumber = $"CRE-{cred.Id.ToString().Substring(0,8).ToUpper()}",
                    Description = "Prepayment / Patient Credit" + (string.IsNullOrEmpty(cred.Reason) ? "" : $" ({cred.Reason})"),
                    Debit = 0,
                    Credit = cred.Amount,
                    SortOrder = 1,
                    SourceId = cred.Id
                });
            }

            // Important: We must calculate balance from the beginning of time up to the end of the statement period.
            // First, filter out transactions AFTER the period.
            var transactionsUpToEnd = allTransactions
                .Where(t => t.TransactionDate.Date <= eDate.Date)
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.SortOrder)
                .ThenBy(t => t.SourceId)
                .ToList();

            decimal runningBal = 0;
            decimal openingBalance = 0;
            var periodTransactions = new List<Clinic.Application.DTOs.Operations.PatientLedgerTransactionDto>();

            foreach (var t in transactionsUpToEnd)
            {
                runningBal = runningBal + t.Debit - t.Credit;
                t.RunningBalance = runningBal;

                if (t.TransactionDate.Date < sDate.Date)
                {
                    // This transaction is before the start date, it contributes to the opening balance
                    openingBalance = runningBal;
                }
                else
                {
                    // This transaction is within the period
                    periodTransactions.Add(t);
                }
            }

            ViewBag.Patient = patient;
            ViewBag.StartDate = sDate;
            ViewBag.EndDate = eDate;
            ViewBag.OpeningBalance = openingBalance;
            ViewBag.ClosingBalance = runningBal;
            ViewBag.PeriodTransactions = periodTransactions;
            ViewBag.PeriodDebit = periodTransactions.Sum(t => t.Debit);
            ViewBag.PeriodCredit = periodTransactions.Sum(t => t.Credit);

            return View("Templates/PatientStatement");
        }
    }
}
