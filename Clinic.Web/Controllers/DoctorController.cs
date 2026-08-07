using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Application.DTOs.MasterData;
using Clinic.Application.UI;
using Clinic.Web.Extensions;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly ISpecialtyService _specialtyService;
        private readonly ILocationService _locationService;

        public DoctorController(IDoctorService doctorService, ISpecialtyService specialtyService, ILocationService locationService)
        {
            _doctorService = doctorService;
            _specialtyService = specialtyService;
            _locationService = locationService;
        }

        [HttpGet]
        [Authorize(Policy = "Doctor.Index")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Doctors / Providers",
                ModuleName = "Master Data",
                Mode = RenderingMode.Template
            };
            var items = await _doctorService.GetAllAsync();
            ViewBag.Meta = meta;
            return View("Templates/Doctor_List", items);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "Doctor.Create")]
        public async Task<IActionResult> Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Doctor",
                ModuleName = "Master Data",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            await PopulateViewBags();
            return View("Templates/Doctor_Form", new DoctorDto());
        }

        [HttpPost("Create")]
        [Authorize(Policy = "Doctor.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorDto dto)
        {
            if (ModelState.IsValid)
            {
                var entity = new Doctor
                {
                    DoctorCode = dto.DoctorCode,
                    Title = dto.Title,
                    FullName = dto.FullName,
                    Gender = dto.Gender,
                    BirthDate = dto.BirthDate,
                    SpecialtyId = dto.SpecialtyId,
                    LicenseNumber = dto.LicenseNumber,
                    RegistrationNumber = dto.RegistrationNumber,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Address = dto.Address,
                    Photo = dto.Photo,
                    Signature = dto.Signature,
                    PrimaryLocationId = dto.PrimaryLocationId,
                    ConsultationDuration = dto.ConsultationDuration,
                    AppointmentInterval = dto.AppointmentInterval,
                    Notes = dto.Notes,
                    IsActive = dto.IsActive,
                    Color = dto.Color
                };
                
                // Automatic deterministic color based on Doctor Id if not provided
                if (string.IsNullOrEmpty(entity.Color))
                {
                    var colors = new[] { "#0d6efd", "#198754", "#dc3545", "#fd7e14", "#6f42c1", "#d63384", "#20c997", "#0dcaf0", "#6610f2", "#e83e8c", "#ffc107" };
                    entity.Color = colors[Math.Abs(entity.Id.GetHashCode()) % colors.Length];
                }

                await _doctorService.CreateAsync(entity);
                return RedirectToAction("Index");
            }
            var meta = new UIMetadata { Title = "Create Doctor", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            await PopulateViewBags(dto.SpecialtyId, dto.PrimaryLocationId);
            return View("Templates/Doctor_Form", dto);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "Doctor.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _doctorService.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var dto = new DoctorDto
            {
                Id = entity.Id,
                DoctorCode = entity.DoctorCode,
                Title = entity.Title,
                FullName = entity.FullName,
                Gender = entity.Gender,
                BirthDate = entity.BirthDate,
                SpecialtyId = entity.SpecialtyId,
                LicenseNumber = entity.LicenseNumber,
                RegistrationNumber = entity.RegistrationNumber,
                Phone = entity.Phone,
                Email = entity.Email,
                Address = entity.Address,
                Photo = entity.Photo,
                Signature = entity.Signature,
                PrimaryLocationId = entity.PrimaryLocationId,
                ConsultationDuration = entity.ConsultationDuration,
                AppointmentInterval = entity.AppointmentInterval,
                Color = entity.Color,
                Notes = entity.Notes,
                IsActive = entity.IsActive,
                Schedules = entity.Schedules.Select(s => new DoctorScheduleDto
                {
                    Id = s.Id,
                    DoctorId = s.DoctorId,
                    LocationId = s.LocationId,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime?.ToString(@"hh\:mm"),
                    EndTime = s.EndTime?.ToString(@"hh\:mm"),
                    BreakStart = s.BreakStart?.ToString(@"hh\:mm"),
                    BreakEnd = s.BreakEnd?.ToString(@"hh\:mm"),
                    MaximumAppointment = s.MaximumAppointment,
                    SlotInterval = s.SlotInterval,
                    IsAvailable = s.IsAvailable
                }).ToList()
            };
            
            var meta = new UIMetadata { Title = "Edit Doctor", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            await PopulateViewBags(dto.SpecialtyId, dto.PrimaryLocationId);
            return View("Templates/Doctor_Form", dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "Doctor.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DoctorDto dto)
        {
            Console.WriteLine($"[FORENSIC] Route ID: {id}");
            Console.WriteLine($"[FORENSIC] DTO ID: {dto.Id}");
            
            if (id != dto.Id) 
            {
                Console.WriteLine($"[FORENSIC] BadRequest triggered: id != dto.Id");
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var entity = await _doctorService.GetByIdAsync(id);
                if (entity == null) return NotFound();

                entity.DoctorCode = dto.DoctorCode;
                entity.Title = dto.Title;
                entity.FullName = dto.FullName;
                entity.Gender = dto.Gender;
                entity.BirthDate = dto.BirthDate;
                entity.SpecialtyId = dto.SpecialtyId;
                entity.LicenseNumber = dto.LicenseNumber;
                entity.RegistrationNumber = dto.RegistrationNumber;
                entity.Phone = dto.Phone;
                entity.Email = dto.Email;
                entity.Address = dto.Address;
                entity.Photo = dto.Photo;
                entity.Signature = dto.Signature;
                entity.PrimaryLocationId = dto.PrimaryLocationId;
                entity.ConsultationDuration = dto.ConsultationDuration;
                entity.AppointmentInterval = dto.AppointmentInterval;
                entity.Notes = dto.Notes;
                entity.IsActive = dto.IsActive;
                entity.Color = dto.Color;

                if (string.IsNullOrEmpty(entity.Color))
                {
                    var colors = new[] { "#0d6efd", "#198754", "#dc3545", "#fd7e14", "#6f42c1", "#d63384", "#20c997", "#0dcaf0", "#6610f2", "#e83e8c", "#ffc107" };
                    entity.Color = colors[Math.Abs(entity.Id.GetHashCode()) % colors.Length];
                }

                await _doctorService.UpdateAsync(entity);
                return RedirectToAction("Index");
            }
            else
            {
                Console.WriteLine("[FORENSIC] ModelState is INVALID!");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"[FORENSIC] Error on {state.Key}: {error.ErrorMessage} {error.Exception?.Message}");
                    }
                }
            }

            var meta = new UIMetadata { Title = "Edit Doctor", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            await PopulateViewBags(dto.SpecialtyId, dto.PrimaryLocationId);
            return View("Templates/Doctor_Form", dto);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "Doctor.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _doctorService.DeleteAsync(id, Guid.Empty);
            return RedirectToAction("Index");
        }

        // --- Schedules ---
        
        [HttpGet("{doctorId}/Schedule/Create")]
        [Authorize(Policy = "Doctor.Schedule")]
        public async Task<IActionResult> CreateSchedule(Guid doctorId)
        {
            var doctor = await _doctorService.GetByIdAsync(doctorId);
            if (doctor == null) return NotFound();

            var meta = new UIMetadata { Title = "Add Schedule - " + doctor.FullName, ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            ViewBag.DoctorId = doctorId;
            await PopulateViewBags();
            return View("Templates/DoctorSchedule_Form", new DoctorScheduleDto { DoctorId = doctorId });
        }

        [HttpPost("{doctorId}/Schedule/Create")]
        [Authorize(Policy = "Doctor.Schedule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule(Guid doctorId, DoctorScheduleDto dto)
        {
            var doctor = await _doctorService.GetByIdAsync(doctorId);
            if (doctor == null) return NotFound();

            ValidateSchedule(dto, doctor);

            if (ModelState.IsValid)
            {
                var schedule = new DoctorSchedule
                {
                    DoctorId = doctorId,
                    LocationId = dto.LocationId,
                    DayOfWeek = dto.DayOfWeek,
                    StartTime = string.IsNullOrEmpty(dto.StartTime) ? null : TimeSpan.Parse(dto.StartTime),
                    EndTime = string.IsNullOrEmpty(dto.EndTime) ? null : TimeSpan.Parse(dto.EndTime),
                    BreakStart = string.IsNullOrEmpty(dto.BreakStart) ? null : TimeSpan.Parse(dto.BreakStart),
                    BreakEnd = string.IsNullOrEmpty(dto.BreakEnd) ? null : TimeSpan.Parse(dto.BreakEnd),
                    MaximumAppointment = dto.MaximumAppointment,
                    SlotInterval = dto.SlotInterval,
                    IsAvailable = dto.IsAvailable
                };
                await _doctorService.CreateScheduleAsync(schedule);
                return RedirectToAction("Edit", new { id = doctorId });
            }
            var meta = new UIMetadata { Title = "Add Schedule", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            ViewBag.DoctorId = doctorId;
            await PopulateViewBags(null, dto.LocationId);
            return View("Templates/DoctorSchedule_Form", dto);
        }

        [HttpGet("{doctorId}/Schedule/Edit/{id}")]
        [Authorize(Policy = "Doctor.Schedule")]
        public async Task<IActionResult> EditSchedule(Guid doctorId, Guid id)
        {
            var doctor = await _doctorService.GetByIdAsync(doctorId);
            if (doctor == null) return NotFound();

            var s = doctor.Schedules.FirstOrDefault(x => x.Id == id);
            if (s == null) return NotFound();

            var dto = new DoctorScheduleDto
            {
                Id = s.Id,
                DoctorId = s.DoctorId,
                LocationId = s.LocationId,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime?.ToString(@"hh\:mm"),
                EndTime = s.EndTime?.ToString(@"hh\:mm"),
                BreakStart = s.BreakStart?.ToString(@"hh\:mm"),
                BreakEnd = s.BreakEnd?.ToString(@"hh\:mm"),
                MaximumAppointment = s.MaximumAppointment,
                SlotInterval = s.SlotInterval,
                IsAvailable = s.IsAvailable
            };

            var meta = new UIMetadata { Title = "Edit Schedule - " + doctor.FullName, ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            ViewBag.DoctorId = doctorId;
            await PopulateViewBags(null, dto.LocationId);
            return View("Templates/DoctorSchedule_Form", dto);
        }

        [HttpPost("{doctorId}/Schedule/Edit/{id}")]
        [Authorize(Policy = "Doctor.Schedule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSchedule(Guid doctorId, Guid id, DoctorScheduleDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var doctor = await _doctorService.GetByIdAsync(doctorId);
            if (doctor == null) return NotFound();

            ValidateSchedule(dto, doctor);

            if (ModelState.IsValid)
            {
                var schedule = doctor.Schedules.FirstOrDefault(x => x.Id == id);
                if (schedule == null) return NotFound();

                schedule.LocationId = dto.LocationId;
                schedule.DayOfWeek = dto.DayOfWeek;
                schedule.StartTime = string.IsNullOrEmpty(dto.StartTime) ? null : TimeSpan.Parse(dto.StartTime);
                schedule.EndTime = string.IsNullOrEmpty(dto.EndTime) ? null : TimeSpan.Parse(dto.EndTime);
                schedule.BreakStart = string.IsNullOrEmpty(dto.BreakStart) ? null : TimeSpan.Parse(dto.BreakStart);
                schedule.BreakEnd = string.IsNullOrEmpty(dto.BreakEnd) ? null : TimeSpan.Parse(dto.BreakEnd);
                schedule.MaximumAppointment = dto.MaximumAppointment;
                schedule.SlotInterval = dto.SlotInterval;
                schedule.IsAvailable = dto.IsAvailable;
                
                await _doctorService.UpdateScheduleAsync(schedule);
                return RedirectToAction("Edit", new { id = doctorId });
            }

            var meta = new UIMetadata { Title = "Edit Schedule", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            ViewBag.DoctorId = doctorId;
            await PopulateViewBags(null, dto.LocationId);
            return View("Templates/DoctorSchedule_Form", dto);
        }

        [HttpPost("Schedule/Delete/{id}")]
        [Authorize(Policy = "Doctor.Schedule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSchedule(Guid id, Guid doctorId)
        {
            await _doctorService.DeleteScheduleAsync(id, Guid.Empty);
            return RedirectToAction("Edit", new { id = doctorId });
        }

        private async Task PopulateViewBags(Guid? selectedSpecialty = null, Guid? selectedLocation = null)
        {
            var specialties = await _specialtyService.GetAllActiveAsync();
            ViewBag.Specialties = specialties.ToSelectList(x => x.Id, x => x.Name, selectedSpecialty);

            var locations = await _locationService.GetAllLocationsAsync();
            ViewBag.Locations = locations.ToSelectList(x => x.Id, x => x.ClinicName, selectedLocation);
        }

        private void ValidateSchedule(DoctorScheduleDto dto, Doctor doctor)
        {
            if (dto.MaximumAppointment < 1) ModelState.AddModelError("MaximumAppointment", "Maximum Appointment must be at least 1.");
            if (dto.SlotInterval <= 0) ModelState.AddModelError("SlotInterval", "Slot Interval must be greater than 0.");
            
            TimeSpan? start = string.IsNullOrEmpty(dto.StartTime) ? null : TimeSpan.Parse(dto.StartTime);
            TimeSpan? end = string.IsNullOrEmpty(dto.EndTime) ? null : TimeSpan.Parse(dto.EndTime);
            TimeSpan? breakStart = string.IsNullOrEmpty(dto.BreakStart) ? null : TimeSpan.Parse(dto.BreakStart);
            TimeSpan? breakEnd = string.IsNullOrEmpty(dto.BreakEnd) ? null : TimeSpan.Parse(dto.BreakEnd);

            if (start != null && end != null && end <= start) ModelState.AddModelError("EndTime", "End time must be after start time.");
            
            if (breakStart != null || breakEnd != null)
            {
                if (breakStart == null || breakEnd == null) ModelState.AddModelError("BreakStart", "Both break start and end must be provided.");
                else 
                {
                    if (breakEnd <= breakStart) ModelState.AddModelError("BreakEnd", "Break end must be after break start.");
                    if (start != null && breakStart < start) ModelState.AddModelError("BreakStart", "Break cannot start before working hours.");
                    if (end != null && breakEnd > end) ModelState.AddModelError("BreakEnd", "Break cannot end after working hours.");
                }
            }

            // Overlap Validation
            if (start != null && end != null && ModelState.IsValid)
            {
                var overlappingSchedule = doctor.Schedules.FirstOrDefault(s => 
                    s.Id != dto.Id && // ignore the schedule being edited
                    !s.IsDeleted && // ignore deleted schedules
                    s.IsAvailable && // ignore inactive schedules
                    s.DayOfWeek == dto.DayOfWeek && 
                    s.StartTime < end && 
                    s.EndTime > start);

                if (overlappingSchedule != null)
                {
                    var locName = overlappingSchedule.Location?.ClinicName ?? "another location";
                    var dayName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)overlappingSchedule.DayOfWeek);
                    var startStr = overlappingSchedule.StartTime?.ToString(@"hh\:mm") ?? "";
                    var endStr = overlappingSchedule.EndTime?.ToString(@"hh\:mm") ?? "";
                    
                    ModelState.AddModelError("", $"Doctor already has another schedule:\nLocation: {locName}\nDay: {dayName}\nStart: {startStr}\nEnd: {endStr}\n\nPlease choose another time.");
                }
            }
        }
    }
}
