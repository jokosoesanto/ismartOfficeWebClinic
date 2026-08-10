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
    public class TreatmentCatalogController : Controller
    {
        private readonly ITreatmentCatalogService _service;
        private readonly ITreatmentCategoryService _categoryService;
        private readonly ITreatmentSubCategoryService _subCategoryService;
        private readonly IMasterReferenceService _masterReferenceService;
        private readonly ICurrentUserService _currentUserService;
        private readonly Clinic.Application.Interfaces.Configuration.ICurrencyService _currencyService;

        public TreatmentCatalogController(
            ITreatmentCatalogService service,
            ITreatmentCategoryService categoryService,
            ITreatmentSubCategoryService subCategoryService,
            IMasterReferenceService masterReferenceService,
            ICurrentUserService currentUserService,
            Clinic.Application.Interfaces.Configuration.ICurrencyService currencyService)
        {
            _service = service;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _masterReferenceService = masterReferenceService;
            _currentUserService = currentUserService;
            _currencyService = currencyService;
        }

        [HttpGet]
        [Authorize(Policy = "MasterData.TreatmentCatalog.View")]
        public async Task<IActionResult> Index(Guid? subCategoryId)
        {
            var meta = new UIMetadata
            {
                Title = "Treatment Catalog",
                ModuleName = "Treatment Catalog",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            ViewBag.SubCategoryId = subCategoryId;
            ViewBag.CurrencySymbol = await _currencyService.GetCurrencySymbolAsync();

            if (subCategoryId.HasValue)
            {
                var subCategory = await _subCategoryService.GetByIdAsync(subCategoryId.Value);
                if (subCategory != null)
                {
                    ViewBag.SubCategoryName = subCategory.SubCategoryName;
                    ViewBag.CategoryId = subCategory.CategoryId;
                    var category = await _categoryService.GetByIdAsync(subCategory.CategoryId);
                    ViewBag.CategoryName = category?.CategoryName;

                    var breadcrumbs = new System.Collections.Generic.List<Clinic.Application.Navigation.NavigationItem>();
                    if (category != null)
                    {
                        breadcrumbs.Add(new Clinic.Application.Navigation.NavigationItem { BreadcrumbTitle = category.CategoryName, Route = $"/TreatmentSubCategory?categoryId={category.Id}" });
                    }
                    breadcrumbs.Add(new Clinic.Application.Navigation.NavigationItem { BreadcrumbTitle = subCategory.SubCategoryName, Route = $"/TreatmentCatalog?subCategoryId={subCategory.Id}" });
                    
                    ViewData["DynamicBreadcrumbs"] = breadcrumbs;
                }
            }

            var data = subCategoryId.HasValue 
                ? await _service.GetBySubCategoryIdAsync(subCategoryId.Value)
                : await _service.GetAllAsync();

            return View(data);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "MasterData.TreatmentCatalog.Create")]
        public async Task<IActionResult> Create(Guid? categoryId, Guid? subCategoryId)
        {
            var meta = new UIMetadata
            {
                Title = "Create Treatment",
                ModuleName = "Treatment Catalog",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            ViewBag.CurrencySymbol = await _currencyService.GetCurrencySymbolAsync();

            var model = new TreatmentCatalogCreateDto { IsActive = true };
            
            if (categoryId.HasValue) model.CategoryId = categoryId.Value;
            if (subCategoryId.HasValue)
            {
                var subCategory = await _subCategoryService.GetByIdAsync(subCategoryId.Value);
                if (subCategory != null)
                {
                    model.CategoryId = subCategory.CategoryId;
                    model.SubCategoryId = subCategoryId.Value;
                }
            }

            await PopulateDropdownsAsync(model.CategoryId, model.SubCategoryId, model.ServiceTypeId);

            return View(model);
        }

        [HttpPost("Create")]
        [Authorize(Policy = "MasterData.TreatmentCatalog.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TreatmentCatalogCreateDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateAsync(model, _currentUserService.UserId ?? Guid.Empty);
                    TempData["SuccessMessage"] = "Treatment created successfully.";
                    return RedirectToAction(nameof(Index), new { subCategoryId = model.SubCategoryId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var meta = new UIMetadata { Title = "Create Treatment", ModuleName = "Treatment Catalog", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            ViewBag.CurrencySymbol = await _currencyService.GetCurrencySymbolAsync();
            await PopulateDropdownsAsync(model.CategoryId, model.SubCategoryId, model.ServiceTypeId);
            
            return View(model);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "MasterData.TreatmentCatalog.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();

            var meta = new UIMetadata
            {
                Title = "Edit Treatment",
                ModuleName = "Treatment Catalog",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            ViewBag.CurrencySymbol = await _currencyService.GetCurrencySymbolAsync();

            var dto = new TreatmentCatalogUpdateDto
            {
                Id = data.Id,
                TreatmentCode = data.TreatmentCode,
                CategoryId = data.CategoryId,
                SubCategoryId = data.SubCategoryId,
                ServiceTypeId = data.ServiceTypeId,
                TreatmentName = data.TreatmentName,
                Description = data.Description,
                DefaultPrice = data.DefaultPrice,
                DurationInMinutes = data.DurationInMinutes,
                RequiresTooth = data.RequiresTooth,
                RequiresSurface = data.RequiresSurface,
                IsActive = data.IsActive
            };

            await PopulateDropdownsAsync(dto.CategoryId, dto.SubCategoryId, dto.ServiceTypeId, true);

            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "MasterData.TreatmentCatalog.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TreatmentCatalogUpdateDto model)
        {
            if (id != model.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(model, _currentUserService.UserId ?? Guid.Empty);
                    TempData["SuccessMessage"] = "Treatment updated successfully.";
                    return RedirectToAction(nameof(Index), new { subCategoryId = model.SubCategoryId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var meta = new UIMetadata { Title = "Edit Treatment", ModuleName = "Treatment Catalog", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            ViewBag.CurrencySymbol = await _currencyService.GetCurrencySymbolAsync();
            await PopulateDropdownsAsync(model.CategoryId, model.SubCategoryId, model.ServiceTypeId, true);
            
            return View(model);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "MasterData.TreatmentCatalog.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid? returnSubCategoryId)
        {
            try
            {
                await _service.DeleteAsync(id, _currentUserService.UserId ?? Guid.Empty);
                TempData["SuccessMessage"] = "Treatment deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { subCategoryId = returnSubCategoryId });
        }

        [HttpGet("GetSubCategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubCategories(Guid categoryId)
        {
            var subs = await _subCategoryService.GetByCategoryIdAsync(categoryId);
            var activeSubs = subs.Where(s => s.IsActive).OrderBy(s => s.SubCategoryName)
                .Select(s => new { value = s.Id, text = s.SubCategoryName });
            return Json(activeSubs);
        }

        private async Task PopulateDropdownsAsync(Guid? selectedCategoryId, Guid? selectedSubCategoryId, Guid? selectedServiceTypeId, bool includeInactive = false)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories
                .Where(c => c.IsActive || (includeInactive && c.Id == selectedCategoryId))
                .OrderBy(c => c.CategoryName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName,
                    Selected = c.Id == selectedCategoryId
                });

            var subCategories = selectedCategoryId.HasValue 
                ? await _subCategoryService.GetByCategoryIdAsync(selectedCategoryId.Value) 
                : new List<TreatmentSubCategoryDto>();

            ViewBag.SubCategories = subCategories
                .Where(s => s.IsActive || (includeInactive && s.Id == selectedSubCategoryId))
                .OrderBy(s => s.SubCategoryName)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.SubCategoryName,
                    Selected = s.Id == selectedSubCategoryId
                });

            var serviceTypes = await _masterReferenceService.GetByCategoryAsync("ServiceType", false);
            ViewBag.ServiceTypes = serviceTypes
                .Where(st => st.IsActive || (includeInactive && st.Id == selectedServiceTypeId))
                .OrderBy(st => st.Name)
                .Select(st => new SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name,
                    Selected = st.Id == selectedServiceTypeId
                });
        }
    }
}
