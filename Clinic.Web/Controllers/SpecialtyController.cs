using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Application.DTOs.MasterData;
using Clinic.Application.UI;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class SpecialtyController : Controller
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtyController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        [HttpGet]
        [Authorize(Policy = "Specialty.Index")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Specialties",
                ModuleName = "Master Data",
                Mode = RenderingMode.Template
            };
            var items = await _specialtyService.GetAllAsync();
            ViewBag.Meta = meta;
            return View("Templates/Specialty_List", items);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "Specialty.Create")]
        public IActionResult Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Specialty",
                ModuleName = "Master Data",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            return View("Templates/Specialty_Form", new SpecialtyDto());
        }

        [HttpPost("Create")]
        [Authorize(Policy = "Specialty.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialtyDto dto)
        {
            if (ModelState.IsValid)
            {
                var entity = new Specialty
                {
                    Code = dto.Code,
                    Name = dto.Name,
                    Description = dto.Description,
                    IsActive = dto.IsActive
                };
                await _specialtyService.CreateAsync(entity);
                return RedirectToAction("Index");
            }
            var meta = new UIMetadata { Title = "Create Specialty", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Specialty_Form", dto);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "Specialty.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _specialtyService.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var dto = new SpecialtyDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
            
            var meta = new UIMetadata { Title = "Edit Specialty", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Specialty_Form", dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "Specialty.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SpecialtyDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var entity = await _specialtyService.GetByIdAsync(id);
                if (entity == null) return NotFound();

                entity.Code = dto.Code;
                entity.Name = dto.Name;
                entity.Description = dto.Description;
                entity.IsActive = dto.IsActive;

                await _specialtyService.UpdateAsync(entity);
                return RedirectToAction("Index");
            }
            var meta = new UIMetadata { Title = "Edit Specialty", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Specialty_Form", dto);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "Specialty.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Assuming current user is fetched correctly in real scenario. For simplicity here passing empty Guid.
            await _specialtyService.DeleteAsync(id, Guid.Empty);
            return RedirectToAction("Index");
        }
    }
}
