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

            if (dto.LeaveDates.Any() && dto.LeaveDates.Min(d => d.Date.Date) <= DateTime.UtcNow.Date)
            {
                TempData["ErrorMessage"] = "Doctor Leave Request cannot be edited because the leave has already started or contains a past leave date.";
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> GetAffectedAppointments(Guid doctorId, [FromBody] System.Collections.Generic.List<DateTime> dates)
        {
            if (doctorId == Guid.Empty || dates == null || !dates.Any())
                return Json(new System.Collections.Generic.List<object>());

            var appointments = await _appointmentService.GetAppointmentsByDoctorAndDatesAsync(doctorId, dates);
            
            var result = appointments.Select(a => new
            {
                date = a.Date.ToString("dd MMM yyyy"),
                time = $"{a.StartTime:hh\\:mm} - {a.EndTime:hh\\:mm}",
                patient = a.PatientName,
                doctor = a.DoctorName,
                chair = a.ChairName ?? "-"
            });

            return Json(result);
        }
    }
}
