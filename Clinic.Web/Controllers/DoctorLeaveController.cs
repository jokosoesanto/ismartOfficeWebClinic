using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Clinic.Application.Interfaces.Operations;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.DTOs.Operations;
using Clinic.Application.UI;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class DoctorLeaveController : Controller
    {
        private readonly IDoctorLeaveRequestService _service;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentService _appointmentService;

        public DoctorLeaveController(
            IDoctorLeaveRequestService service,
            IDoctorRepository doctorRepository,
            IAppointmentService appointmentService)
        {
            _service = service;
            _doctorRepository = doctorRepository;
            _appointmentService = appointmentService;
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }

        [HttpGet]
        [Authorize(Policy = "DoctorLeave.View")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Doctor Leave",
                ModuleName = "DoctorLeave",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var model = await _service.GetAllAsync();
            return View(model);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "DoctorLeave.Create")]
        public async Task<IActionResult> Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Doctor Leave",
                ModuleName = "DoctorLeave",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            await PrepareDoctorDropdownAsync();
            return View(new DoctorLeaveRequestDto());
        }

        [HttpPost("Create")]
        [Authorize(Policy = "DoctorLeave.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorLeaveRequestDto dto)
        {
            // Remove auto-validation on LeaveDates since they come via hidden fields
            ModelState.Remove("LeaveDates");

            if (!ModelState.IsValid)
            {
                var meta = new UIMetadata { Title = "Create Doctor Leave", ModuleName = "DoctorLeave", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                await PrepareDoctorDropdownAsync(dto.DoctorId);
                return View(dto);
            }

            try
            {
                await _service.CreateAsync(dto, GetCurrentUserId());
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var meta = new UIMetadata { Title = "Create Doctor Leave", ModuleName = "DoctorLeave", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                await PrepareDoctorDropdownAsync(dto.DoctorId);
                return View(dto);
            }
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "DoctorLeave.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            bool isImmutable = dto.LeaveDateDetails.Any(d => d.Date.Date <= DateTime.UtcNow.Date);
            ViewBag.IsImmutable = isImmutable;
            if (isImmutable)
            {
                ViewBag.ImmutableMessage = "This leave request has already started. Normal edit is disabled, but you can cancel individual future dates below.";
            }

            var meta = new UIMetadata
            {
                Title = "Edit Doctor Leave",
                ModuleName = "DoctorLeave",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            await PrepareDoctorDropdownAsync(dto.DoctorId);
            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "DoctorLeave.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DoctorLeaveRequestDto dto)
        {
            if (id != dto.Id) return BadRequest();

            ModelState.Remove("LeaveDates");

            if (!ModelState.IsValid)
            {
                var meta = new UIMetadata { Title = "Edit Doctor Leave", ModuleName = "DoctorLeave", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                await PrepareDoctorDropdownAsync(dto.DoctorId);
                return View(dto);
            }

            try
            {
                await _service.UpdateAsync(dto, GetCurrentUserId());
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var meta = new UIMetadata { Title = "Edit Doctor Leave", ModuleName = "DoctorLeave", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                await PrepareDoctorDropdownAsync(dto.DoctorId);
                return View(dto);
            }
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "DoctorLeave.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id, GetCurrentUserId());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("CancelDate/{leaveDateId}")]
        [Authorize(Policy = "DoctorLeave.Edit")]
        public async Task<IActionResult> CancelDate(Guid leaveDateId, [FromForm] string? reason)
        {
            try
            {
                await _service.CancelLeaveDateAsync(leaveDateId, reason, GetCurrentUserId());
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task PrepareDoctorDropdownAsync(Guid? selectedDoctorId = null)
        {
            var doctors = await _doctorRepository.GetAllActiveAsync();
            var doctorList = doctors.OrderBy(d => d.FullName).ToList();
            ViewBag.Doctors = new SelectList(doctorList, "Id", "FullName", selectedDoctorId);
        }

        [HttpPost("GetAffectedAppointments")]
        [Authorize]
        public async Task<IActionResult> GetAffectedAppointments(Guid doctorId, [FromBody] AffectedAppointmentsRequest req)
        {
            var dates = req?.Dates ?? new System.Collections.Generic.List<DateTime>();
            var knownIds = req?.KnownAppointmentIds ?? new System.Collections.Generic.List<Guid>();

            if (doctorId == Guid.Empty || !dates.Any())
                return Json(new System.Collections.Generic.List<object>());

            var activeAppointments = (await _appointmentService.GetAppointmentsByDoctorAndDatesAsync(doctorId, dates)).ToList();
            var activeIds = activeAppointments.Select(a => a.Id).ToHashSet();

            var result = new System.Collections.Generic.List<object>();

            // 1. Add all active/unresolved appointments
            foreach (var a in activeAppointments)
            {
                result.Add(new
                {
                    id = a.Id,
                    isoDate = a.Date.ToString("yyyy-MM-dd"),
                    date = a.Date.ToString("dd MMM yyyy"),
                    time = $"{a.StartTime:hh\\:mm} - {a.EndTime:hh\\:mm}",
                    patient = a.PatientName,
                    doctor = a.DoctorName,
                    chair = a.ChairName ?? "-",
                    status = "UNRESOLVED"
                });
            }

            // 2. Add resolved appointments (previously known but no longer active conflicts)
            var resolvedIds = knownIds.Where(id => !activeIds.Contains(id)).ToList();
            foreach (var id in resolvedIds)
            {
                var appt = await _appointmentService.GetByIdAsync(id);
                if (appt != null)
                {
                    result.Add(new
                    {
                        id = appt.Id,
                        isoDate = appt.Date.ToString("yyyy-MM-dd"),
                        date = appt.Date.ToString("dd MMM yyyy"),
                        time = $"{appt.StartTime:hh\\:mm} - {appt.EndTime:hh\\:mm}",
                        patient = appt.PatientName,
                        doctor = appt.DoctorName,
                        chair = appt.ChairName ?? "-",
                        status = "RESOLVED"
                    });
                }
            }

            return Json(result);
        }
    }

    public class AffectedAppointmentsRequest
    {
        public System.Collections.Generic.List<DateTime> Dates { get; set; } = new System.Collections.Generic.List<DateTime>();
        public System.Collections.Generic.List<Guid> KnownAppointmentIds { get; set; } = new System.Collections.Generic.List<Guid>();
    }
}
