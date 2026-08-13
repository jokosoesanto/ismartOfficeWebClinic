using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.DTOs.MasterData;
using Clinic.Application.UI;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        [Authorize(Policy = "Location.Index")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Locations",
                ModuleName = "Master Data",
                Mode = RenderingMode.Template
            };
            var items = await _locationService.GetAllLocationsAsync();
            ViewBag.Meta = meta;
            return View("Templates/Location_List", items);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "Location.Create")]
        public IActionResult Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Location",
                ModuleName = "Master Data",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            return View("Templates/Location_Form", new LocationDto());
        }

        [HttpPost("Create")]
        [Authorize(Policy = "Location.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationDto dto)
        {
            if (ModelState.IsValid)
            {
                Guid? currentUserId = null;
                if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                    currentUserId = uid;

                await _locationService.SaveLocationAsync(dto, currentUserId);
                return RedirectToAction("Index");
            }
            var meta = new UIMetadata { Title = "Create Location", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Location_Form", dto);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "Location.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _locationService.GetLocationByIdAsync(id);
            if (dto == null) return NotFound();
            
            var meta = new UIMetadata { Title = "Edit Location", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Location_Form", dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "Location.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LocationDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                Guid? currentUserId = null;
                if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                    currentUserId = uid;

                await _locationService.SaveLocationAsync(dto, currentUserId);
                return RedirectToAction("Index");
            }
            var meta = new UIMetadata { Title = "Edit Location", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Location_Form", dto);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "Location.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _locationService.DeleteLocationAsync(id, currentUserId);
            return RedirectToAction("Index");
        }
    }
}
