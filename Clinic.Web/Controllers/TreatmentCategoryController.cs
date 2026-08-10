using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.Interfaces.Auth;
using Clinic.Application.UI;
using Clinic.Domain.Entities.MasterData;
using System.Linq;
using Clinic.Application.DTOs.MasterData;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class TreatmentCategoryController : Controller
    {
        private readonly ITreatmentCategoryService _service;
        private readonly ICurrentUserService _currentUserService;

        public TreatmentCategoryController(
            ITreatmentCategoryService service,
            ICurrentUserService currentUserService)
        {
            _service = service;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize(Policy = "MasterData.TreatmentCategory.View")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Treatment Category",
                ModuleName = "Treatment Category",
                Mode = RenderingMode.Template
            };

            var data = await _service.GetAllAsync();
            ViewBag.Meta = meta;
            return View(data);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "MasterData.TreatmentCategory.Create")]
        public IActionResult Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Treatment Category",
                ModuleName = "Treatment Category",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            
            return View(new TreatmentCategory { IsActive = true });
        }

        [HttpPost("Create")]
        [Authorize(Policy = "MasterData.TreatmentCategory.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TreatmentCategory model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateAsync(model, _currentUserService.UserId ?? Guid.Empty);
                    TempData["SuccessMessage"] = "Treatment Category created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var meta = new UIMetadata { Title = "Create Treatment Category", ModuleName = "Treatment Category", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View(model);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "MasterData.TreatmentCategory.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }

            var meta = new UIMetadata
            {
                Title = "Edit Treatment Category",
                ModuleName = "Treatment Category",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var dto = new TreatmentCategoryUpdateDto
            {
                Id = data.Id,
                CategoryCode = data.CategoryCode,
                CategoryName = data.CategoryName,
                Description = data.Description,
                DisplayOrder = data.DisplayOrder,
                IsActive = data.IsActive
            };

            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "MasterData.TreatmentCategory.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TreatmentCategoryUpdateDto model)
        {
            if (id != model.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(model, _currentUserService.UserId ?? Guid.Empty);
                    TempData["SuccessMessage"] = "Treatment Category updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var meta = new UIMetadata { Title = "Edit Treatment Category", ModuleName = "Treatment Category", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View(model);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "MasterData.TreatmentCategory.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id, _currentUserService.UserId ?? Guid.Empty);
                TempData["SuccessMessage"] = "Treatment Category deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
