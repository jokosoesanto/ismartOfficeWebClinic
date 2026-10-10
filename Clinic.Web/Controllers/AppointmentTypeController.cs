using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.UI;
using Clinic.Application.DTOs.MasterData;
using Clinic.Domain.Enums;
using System.Security.Claims;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AppointmentTypeController : Controller
    {
        private readonly IAppointmentTypeService _service;

        public AppointmentTypeController(IAppointmentTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "MasterData.AppointmentType.View")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Appointment Type Master",
                ModuleName = "Appointment Type",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var model = await _service.GetAllAsync();
            return View(model);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "MasterData.AppointmentType.Create")]
        public IActionResult Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Appointment Type",
                ModuleName = "Appointment Type",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            return View(new AppointmentTypeCreateDto { IsActive = true, Complexity = AppointmentComplexity.Medium });
        }

        [HttpPost("Create")]
        [Authorize(Policy = "MasterData.AppointmentType.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentTypeCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var meta = new UIMetadata { Title = "Create Appointment Type", ModuleName = "Appointment Type", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                return View(dto);
            }

            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Guid? userId = string.IsNullOrEmpty(userIdStr) ? null : Guid.Parse(userIdStr);
                
                await _service.CreateAsync(dto, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var meta = new UIMetadata { Title = "Create Appointment Type", ModuleName = "Appointment Type", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                return View(dto);
            }
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "MasterData.AppointmentType.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var dto = new AppointmentTypeCreateDto
            {
                Code = entity.Code,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                Complexity = entity.Complexity,
                DurationInMinutes = entity.DurationInMinutes,
                IsActive = entity.IsActive
            };

            var meta = new UIMetadata
            {
                Title = "Edit Appointment Type",
                ModuleName = "Appointment Type",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "MasterData.AppointmentType.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AppointmentTypeCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var meta = new UIMetadata { Title = "Edit Appointment Type", ModuleName = "Appointment Type", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                return View(dto);
            }

            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Guid? userId = string.IsNullOrEmpty(userIdStr) ? null : Guid.Parse(userIdStr);
                
                await _service.UpdateAsync(id, dto, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var meta = new UIMetadata { Title = "Edit Appointment Type", ModuleName = "Appointment Type", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                return View(dto);
            }
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "MasterData.AppointmentType.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Guid? userId = string.IsNullOrEmpty(userIdStr) ? null : Guid.Parse(userIdStr);
                
                await _service.DeleteAsync(id, userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
