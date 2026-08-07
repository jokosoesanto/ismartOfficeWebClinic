using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Domain.Entities.System;
using Microsoft.AspNetCore.Authorization;

namespace Clinic.Web.Controllers
{
    [Authorize]
    public class MasterReferenceController : Controller
    {
        private readonly IMasterReferenceService _masterReferenceService;

        public MasterReferenceController(IMasterReferenceService masterReferenceService)
        {
            _masterReferenceService = masterReferenceService;
        }

        [Authorize(Policy = "MasterReference.View")]
        public async Task<IActionResult> Index()
        {
            var categories = await _masterReferenceService.GetCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        [Authorize(Policy = "MasterReference.View")]
        public async Task<IActionResult> GetList(string category)
        {
            var data = await _masterReferenceService.GetByCategoryAsync(category, activeOnly: false);
            return Json(new { data });
        }

        [HttpGet]
        [Authorize(Policy = "MasterReference.Create")]
        public async Task<IActionResult> Create(string category)
        {
            var model = new MasterReference { Category = category, IsActive = true };
            return PartialView("_Form", model);
        }

        [HttpPost]
        [Authorize(Policy = "MasterReference.Create")]
        public async Task<IActionResult> Create(MasterReference model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }

            try
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Guid userId = string.IsNullOrEmpty(userIdStr) ? Guid.Empty : Guid.Parse(userIdStr);

                await _masterReferenceService.CreateAsync(model, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Policy = "MasterReference.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _masterReferenceService.GetByIdAsync(id);
            if (model == null) return NotFound();
            return PartialView("_Form", model);
        }

        [HttpPost]
        [Authorize(Policy = "MasterReference.Edit")]
        public async Task<IActionResult> Edit(MasterReference model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }

            try
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Guid userId = string.IsNullOrEmpty(userIdStr) ? Guid.Empty : Guid.Parse(userIdStr);

                await _masterReferenceService.UpdateAsync(model, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "MasterReference.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Guid userId = string.IsNullOrEmpty(userIdStr) ? Guid.Empty : Guid.Parse(userIdStr);

                await _masterReferenceService.DeleteAsync(id, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
