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
    public class ChairController : Controller
    {
        private readonly IChairService _chairService;
        private readonly ILocationService _locationService;

        public ChairController(IChairService chairService, ILocationService locationService)
        {
            _chairService = chairService;
            _locationService = locationService;
        }

        [HttpGet]
        [Authorize(Policy = "Chair.Index")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Chairs",
                ModuleName = "Master Data",
                Mode = RenderingMode.Template
            };
            var items = await _chairService.GetAllChairsAsync();
            ViewBag.Meta = meta;
            return View("Templates/Chair_List", items);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "Chair.Create")]
        public async Task<IActionResult> Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Chair",
                ModuleName = "Master Data",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            ViewBag.Locations = await _locationService.GetAllLocationsAsync();
            return View("Templates/Chair_Form", new ChairDto());
        }

        [HttpPost("Create")]
        [Authorize(Policy = "Chair.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChairDto dto)
        {
            ModelState.Remove("Id");
            ModelState.Remove("LocationName");
            ModelState.Remove("Description");
            if (ModelState.IsValid)
            {
                Guid? currentUserId = null;
                if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                    currentUserId = uid;

                await _chairService.SaveChairAsync(dto, currentUserId);
                return RedirectToAction("Index");
            }

            var meta = new UIMetadata { Title = "Create Chair", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            ViewBag.Locations = await _locationService.GetAllLocationsAsync();
            return View("Templates/Chair_Form", dto);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "Chair.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _chairService.GetChairByIdAsync(id);
            if (dto == null) return NotFound();
            
            var meta = new UIMetadata { Title = "Edit Chair", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            ViewBag.Locations = await _locationService.GetAllLocationsAsync();
            return View("Templates/Chair_Form", dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "Chair.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ChairDto dto)
        {
            if (id != dto.Id) return BadRequest();

            ModelState.Remove("LocationName");
            ModelState.Remove("Description");
            if (ModelState.IsValid)
            {
                Guid? currentUserId = null;
                if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                    currentUserId = uid;

                await _chairService.SaveChairAsync(dto, currentUserId);
                return RedirectToAction("Index");
            }
            var meta = new UIMetadata { Title = "Edit Chair", ModuleName = "Master Data", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            ViewBag.Locations = await _locationService.GetAllLocationsAsync();
            return View("Templates/Chair_Form", dto);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "Chair.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid? currentUserId = null;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid))
                currentUserId = uid;

            await _chairService.DeleteChairAsync(id, currentUserId);
            return RedirectToAction("Index");
        }
    }
}
