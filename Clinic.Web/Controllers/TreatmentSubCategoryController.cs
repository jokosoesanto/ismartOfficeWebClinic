using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.Interfaces.Auth;
using Clinic.Application.UI;
using Clinic.Application.DTOs.MasterData;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class TreatmentSubCategoryController : Controller
    {
        private readonly ITreatmentSubCategoryService _service;
        private readonly ITreatmentCategoryService _categoryService;
        private readonly ICurrentUserService _currentUserService;

        public TreatmentSubCategoryController(
            ITreatmentSubCategoryService service,
            ITreatmentCategoryService categoryService,
            ICurrentUserService currentUserService)
        {
            _service = service;
            _categoryService = categoryService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize(Policy = "MasterData.TreatmentSubCategory.View")]
        public async Task<IActionResult> Index(Guid? categoryId)
        {
            var meta = new UIMetadata
            {
                Title = "Treatment SubCategories",
                ModuleName = "Treatment SubCategory",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            ViewBag.CategoryId = categoryId;

            if (categoryId.HasValue)
            {
                var category = await _categoryService.GetByIdAsync(categoryId.Value);
                if (category != null)
                {
                    ViewBag.CategoryName = category.CategoryName;
                    ViewData["DynamicBreadcrumbs"] = new System.Collections.Generic.List<Clinic.Application.Navigation.NavigationItem>
                    {
                        new Clinic.Application.Navigation.NavigationItem { BreadcrumbTitle = category.CategoryName, Route = $"/TreatmentSubCategory?categoryId={categoryId.Value}" }
                    };
                }
            }

            var data = categoryId.HasValue 
                ? await _service.GetByCategoryIdAsync(categoryId.Value)
                : await _service.GetAllAsync();

            return View(data);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "MasterData.TreatmentSubCategory.Create")]
        public async Task<IActionResult> Create(Guid? categoryId)
        {
            var meta = new UIMetadata
            {
                Title = "Create Treatment SubCategory",
                ModuleName = "Treatment SubCategory",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var model = new TreatmentSubCategoryCreateDto { IsActive = true };
            if (categoryId.HasValue)
            {
                model.CategoryId = categoryId.Value;
            }

            await PopulateCategoryDropdownAsync(model.CategoryId);

            return View(model);
        }

        [HttpPost("Create")]
        [Authorize(Policy = "MasterData.TreatmentSubCategory.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TreatmentSubCategoryCreateDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateAsync(model, _currentUserService.UserId ?? Guid.Empty);
                    TempData["SuccessMessage"] = "Treatment SubCategory created successfully.";
                    return RedirectToAction(nameof(Index), new { categoryId = model.CategoryId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var meta = new UIMetadata { Title = "Create Treatment SubCategory", ModuleName = "Treatment SubCategory", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            await PopulateCategoryDropdownAsync(model.CategoryId);
            
            return View(model);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "MasterData.TreatmentSubCategory.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }

            var meta = new UIMetadata
            {
                Title = "Edit Treatment SubCategory",
                ModuleName = "Treatment SubCategory",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var dto = new TreatmentSubCategoryUpdateDto
            {
                Id = data.Id,
                CategoryId = data.CategoryId,
                SubCategoryCode = data.SubCategoryCode,
                SubCategoryName = data.SubCategoryName,
                Description = data.Description,
                DisplayOrder = data.DisplayOrder,
                IsActive = data.IsActive
            };

            await PopulateCategoryDropdownAsync(dto.CategoryId, true);

            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "MasterData.TreatmentSubCategory.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TreatmentSubCategoryUpdateDto model)
        {
            if (id != model.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(model, _currentUserService.UserId ?? Guid.Empty);
                    TempData["SuccessMessage"] = "Treatment SubCategory updated successfully.";
                    return RedirectToAction(nameof(Index), new { categoryId = model.CategoryId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var meta = new UIMetadata { Title = "Edit Treatment SubCategory", ModuleName = "Treatment SubCategory", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            await PopulateCategoryDropdownAsync(model.CategoryId, true);
            
            return View(model);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "MasterData.TreatmentSubCategory.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid? returnCategoryId)
        {
            try
            {
                await _service.DeleteAsync(id, _currentUserService.UserId ?? Guid.Empty);
                TempData["SuccessMessage"] = "Treatment SubCategory deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { categoryId = returnCategoryId });
        }

        private async Task PopulateCategoryDropdownAsync(Guid? selectedId = null, bool includeInactiveOfSelected = false)
        {
            var allCategories = await _categoryService.GetAllAsync();
            
            var selectableCategories = allCategories
                .Where(c => c.IsActive || (includeInactiveOfSelected && c.Id == selectedId))
                .OrderBy(c => c.CategoryName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName,
                    Selected = c.Id == selectedId
                });

            ViewBag.Categories = selectableCategories;
        }
    }
}
