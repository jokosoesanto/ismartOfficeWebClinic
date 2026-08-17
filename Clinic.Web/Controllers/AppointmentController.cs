using System;
using System.Linq;
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
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly ILocationService _locationService;
        private readonly IChairService _chairService;

        public AppointmentController(
            IAppointmentService appointmentService,
            IPatientService patientService,
            IDoctorService doctorService,
            ILocationService locationService,
            IChairService chairService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
            _locationService = locationService;
            _chairService = chairService;
        }

        [HttpGet]
        [Authorize(Policy = "Appointment.Index")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Appointments",
                ModuleName = "Operations",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var appointments = await _appointmentService.GetAllAsync();
            return View(appointments);
        }

        [HttpGet("Details/{id}")]
        [Authorize(Policy = "Appointment.Index")]
        public async Task<IActionResult> Details(Guid id)
        {
            var meta = new UIMetadata
            {
                Title = "Appointment Details",
                ModuleName = "Operations",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var dto = await _appointmentService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "Appointment.Create")]
        public async Task<IActionResult> Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Appointment",
                ModuleName = "Operations",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            await PopulateDropdownsAsync();

            var dto = new AppointmentDto 
            { 
                Date = DateTime.Today,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            };
            return View(dto);
        }

        [HttpPost("Create")]
        [Authorize(Policy = "Appointment.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid? currentUserId = null;
                    if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                        currentUserId = uid;

                    await _appointmentService.CreateAsync(dto, currentUserId ?? Guid.Empty);
                    TempData["SuccessMessage"] = "Appointment created successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var meta = new UIMetadata
            {
                Title = "Create Appointment",
                ModuleName = "Operations",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            await PopulateDropdownsAsync();
            
            return View(dto);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "Appointment.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var meta = new UIMetadata
            {
                Title = "Edit Appointment",
                ModuleName = "Operations",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var dto = await _appointmentService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            await PopulateDropdownsAsync();
            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "Appointment.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AppointmentDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    Guid? currentUserId = null;
                    if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                        currentUserId = uid;

                    await _appointmentService.UpdateAsync(dto, currentUserId ?? Guid.Empty);
                    TempData["SuccessMessage"] = "Appointment updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var meta = new UIMetadata
            {
                Title = "Edit Appointment",
                ModuleName = "Operations",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            await PopulateDropdownsAsync();
            
            return View(dto);
        }
        [HttpGet("Reassign/{id}")]
        [Authorize(Policy = "Appointment.Edit")]
        public async Task<IActionResult> Reassign(Guid id)
        {
            var meta = new UIMetadata
            {
                Title = "Reassign Doctor",
                ModuleName = "Operations",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var dto = await _appointmentService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var eligibleIds = await _appointmentService.GetEligibleDoctorIdsForReassignmentAsync(id);
            var allDoctors = await _doctorService.GetAllAsync();
            var validDoctors = allDoctors.Where(d => eligibleIds.Contains(d.Id)).ToList();
            ViewBag.Doctors = new SelectList(validDoctors, "Id", "FullName");

            return View(dto);
        }

        [HttpPost("Reassign/{id}")]
        [Authorize(Policy = "Appointment.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reassign(Guid id, AppointmentDto dto)
        {
            if (id != dto.Id) return BadRequest();

            // Retrieve original appointment to ensure read-only fields haven't been tampered with
            var originalDto = await _appointmentService.GetByIdAsync(id);
            if (originalDto == null) return NotFound();

            var originalDoctorId = originalDto.DoctorId;

            // Reconstruct DTO with only the intended changes
            originalDto.DoctorId = dto.DoctorId;
            originalDto.Notes = dto.Notes;

            ModelState.Clear(); 
            if (TryValidateModel(originalDto))
            {
                try
                {
                    Guid? currentUserId = null;
                    if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                        currentUserId = uid;

                    await _appointmentService.UpdateAsync(originalDto, currentUserId ?? Guid.Empty);
                    TempData["SuccessMessage"] = "Doctor reassigned successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var meta = new UIMetadata
            {
                Title = "Reassign Doctor",
                ModuleName = "Operations",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            
            var eligibleIds = await _appointmentService.GetEligibleDoctorIdsForReassignmentAsync(id);
            var allDoctors = await _doctorService.GetAllAsync();
            var validDoctors = allDoctors.Where(d => eligibleIds.Contains(d.Id)).ToList();
            ViewBag.Doctors = new SelectList(validDoctors, "Id", "FullName");
            
            return View(originalDto);
        }


        [HttpPost("Reschedule")]
        [Authorize(Policy = "Appointment.Edit")]
        public async Task<IActionResult> Reschedule(Guid id, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var dto = await _appointmentService.GetByIdAsync(id);
                if (dto == null) return Json(new { success = false, message = "Appointment not found." });

                dto.Date = date;
                dto.StartTime = startTime;
                dto.EndTime = endTime;

                Guid? currentUserId = null;
                if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                    currentUserId = uid;

                await _appointmentService.UpdateAsync(dto, currentUserId ?? Guid.Empty);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetChairsByLocation/{locationId}")]
        [Authorize]
        public async Task<IActionResult> GetChairsByLocation(Guid locationId)
        {
            var allChairs = await _chairService.GetAllChairsAsync();
            var filteredChairs = allChairs.Where(c => c.LocationId == locationId).Select(c => new {
                value = c.Id,
                text = c.Name
            }).ToList();
            return Json(filteredChairs);
        }

        private async Task PopulateDropdownsAsync()
        {
            var patients = await _patientService.GetAllAsync();
            var doctors = await _doctorService.GetAllAsync();
            var locations = await _locationService.GetAllLocationsAsync();

            ViewBag.Patients = new SelectList(patients, "Id", "FullName");
            ViewBag.Doctors = new SelectList(doctors, "Id", "FullName");
            ViewBag.Locations = new SelectList(locations, "Id", "ClinicName");
            // Chair will be dynamically loaded in UI, provide an empty list initially
            ViewBag.Chairs = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "Appointment.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _appointmentService.DeleteAsync(id, currentUserId ?? Guid.Empty);
            TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
