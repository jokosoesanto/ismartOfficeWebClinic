using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.UI;
using Clinic.Application.DTOs.MasterData;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class DiagnosisMasterController : Controller
    {
        private readonly IDiagnosisMasterService _service;

        public DiagnosisMasterController(IDiagnosisMasterService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "MasterData.DiagnosisMaster.View")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Diagnosis Master",
                ModuleName = "Diagnosis Master",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var model = await _service.GetAllAsync();
            return View(model);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "MasterData.DiagnosisMaster.Create")]
        public IActionResult Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Diagnosis",
                ModuleName = "Diagnosis Master",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            return View(new DiagnosisMasterCreateDto { IsActive = true });
        }

        [HttpPost("Create")]
        [Authorize(Policy = "MasterData.DiagnosisMaster.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiagnosisMasterCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var meta = new UIMetadata { Title = "Create Diagnosis", ModuleName = "Diagnosis Master", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                return View(dto);
            }

            try
            {
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var meta = new UIMetadata { Title = "Create Diagnosis", ModuleName = "Diagnosis Master", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                return View(dto);
            }
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "MasterData.DiagnosisMaster.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var dto = new DiagnosisMasterCreateDto
            {
                DiagnosisCode = entity.DiagnosisCode,
                DiagnosisName = entity.DiagnosisName,
                Description = entity.Description,
                IsActive = entity.IsActive
            };

            var meta = new UIMetadata
            {
                Title = "Edit Diagnosis",
                ModuleName = "Diagnosis Master",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "MasterData.DiagnosisMaster.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DiagnosisMasterCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var meta = new UIMetadata { Title = "Edit Diagnosis", ModuleName = "Diagnosis Master", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                return View(dto);
            }

            try
            {
                await _service.UpdateAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var meta = new UIMetadata { Title = "Edit Diagnosis", ModuleName = "Diagnosis Master", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                return View(dto);
            }
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "MasterData.DiagnosisMaster.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
