using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.DTOs.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Application.UI;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class SupplierController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        [HttpGet]
        [Authorize(Policy = "Supplier.Index")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Supplier Management",
                ModuleName = "Inventory",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            var suppliers = await _supplierRepository.GetAllAsync(includeDeleted: false);
            var dtos = suppliers.Select(s => new SupplierDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                IsActive = s.IsActive
            }).OrderBy(x => x.Name).ToList();

            return View("Templates/Supplier_List", dtos);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "Supplier.Create")]
        public IActionResult Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Supplier",
                ModuleName = "Inventory",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            return View("Templates/Supplier_Form", new SupplierDto());
        }

        [HttpPost("Create")]
        [Authorize(Policy = "Supplier.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierDto dto)
        {
            if (ModelState.IsValid)
            {
                var existing = await _supplierRepository.GetByCodeAsync(dto.Code);
                if (existing != null)
                {
                    ModelState.AddModelError("Code", "Supplier Code already exists.");
                }
                else
                {
                    var entity = new Supplier
                    {
                        Code = dto.Code,
                        Name = dto.Name,
                        ContactPerson = dto.ContactPerson,
                        Phone = dto.Phone,
                        Email = dto.Email,
                        Address = dto.Address,
                        IsActive = dto.IsActive
                    };

                    await _supplierRepository.AddAsync(entity);
                    TempData["Success"] = "Supplier created successfully.";
                    return RedirectToAction("Index");
                }
            }

            var meta = new UIMetadata { Title = "Create Supplier", ModuleName = "Inventory", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Supplier_Form", dto);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "Supplier.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _supplierRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return NotFound();

            var dto = new SupplierDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                ContactPerson = entity.ContactPerson,
                Phone = entity.Phone,
                Email = entity.Email,
                Address = entity.Address,
                IsActive = entity.IsActive
            };

            var meta = new UIMetadata { Title = "Edit Supplier", ModuleName = "Inventory", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Supplier_Form", dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "Supplier.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SupplierDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var entity = await _supplierRepository.GetByIdAsync(id);
                if (entity == null || entity.IsDeleted) return NotFound();

                var existingCode = await _supplierRepository.GetByCodeAsync(dto.Code);
                if (existingCode != null && existingCode.Id != id)
                {
                    ModelState.AddModelError("Code", "Supplier Code already exists.");
                }
                else
                {
                    entity.Code = dto.Code;
                    entity.Name = dto.Name;
                    entity.ContactPerson = dto.ContactPerson;
                    entity.Phone = dto.Phone;
                    entity.Email = dto.Email;
                    entity.Address = dto.Address;
                    entity.IsActive = dto.IsActive;

                    await _supplierRepository.UpdateAsync(entity);
                    TempData["Success"] = "Supplier updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            var meta = new UIMetadata { Title = "Edit Supplier", ModuleName = "Inventory", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Supplier_Form", dto);
        }

        [HttpPost("ToggleStatus/{id}")]
        [Authorize(Policy = "Supplier.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var entity = await _supplierRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return NotFound();

            entity.IsActive = !entity.IsActive;
            await _supplierRepository.UpdateAsync(entity);
            
            TempData["Success"] = $"Supplier {(entity.IsActive ? "activated" : "deactivated")} successfully.";
            return RedirectToAction("Index");
        }
    }
}
