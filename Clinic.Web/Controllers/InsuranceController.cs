using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.UI;
using Clinic.Application.DTOs.MasterData;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class InsuranceController : Controller
    {
        private readonly IInsuranceService _service;
        private readonly IMasterReferenceService _masterReferenceService;

        public InsuranceController(
            IInsuranceService service,
            IMasterReferenceService masterReferenceService)
        {
            _service = service;
            _masterReferenceService = masterReferenceService;
        }

        [HttpGet]
        [Authorize(Policy = "MasterData.Insurance.View")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Insurance",
                ModuleName = "Insurance",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var model = await _service.GetAllAsync();
            return View(model);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "MasterData.Insurance.Create")]
        public async Task<IActionResult> Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Insurance",
                ModuleName = "Insurance",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            await PrepareDropdownsAsync();
            return View(new InsuranceCreateEditDto { IsActive = true });
        }

        [HttpPost("Create")]
        [Authorize(Policy = "MasterData.Insurance.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InsuranceCreateEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                var meta = new UIMetadata { Title = "Create Insurance", ModuleName = "Insurance", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                await PrepareDropdownsAsync();
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
                var meta = new UIMetadata { Title = "Create Insurance", ModuleName = "Insurance", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                await PrepareDropdownsAsync();
                return View(dto);
            }
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "MasterData.Insurance.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetForEditAsync(id);
            if (dto == null) return NotFound();

            var meta = new UIMetadata
            {
                Title = "Edit Insurance",
                ModuleName = "Insurance",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            await PrepareDropdownsAsync(dto.GroupId);
            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "MasterData.Insurance.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, InsuranceCreateEditDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                var meta = new UIMetadata { Title = "Edit Insurance", ModuleName = "Insurance", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                await PrepareDropdownsAsync(dto.GroupId);
                return View(dto);
            }

            try
            {
                await _service.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var meta = new UIMetadata { Title = "Edit Insurance", ModuleName = "Insurance", Mode = RenderingMode.Template };
                ViewBag.Meta = meta;
                await PrepareDropdownsAsync(dto.GroupId);
                return View(dto);
            }
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "MasterData.Insurance.Delete")]
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

        private async Task PrepareDropdownsAsync(Guid? selectedGroupId = null)
        {
            var groups = await _masterReferenceService.GetByCategoryAsync("InsuranceGroup");
            // If the currently selected group is no longer active, we must still allow it in the list (for edit)
            var activeGroups = groups.Where(g => g.IsActive || g.Id == selectedGroupId).OrderBy(g => g.Name).ToList();
            ViewBag.InsuranceGroups = new SelectList(activeGroups, "Id", "Name");
        }
    }
}
