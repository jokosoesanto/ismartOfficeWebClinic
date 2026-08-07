using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.Interfaces.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Clinic.Web.Controllers
{
    [Authorize]
    public class NumberSequenceController : Controller
    {
        private readonly INumberSequenceService _numberSequenceService;

        public NumberSequenceController(INumberSequenceService numberSequenceService)
        {
            _numberSequenceService = numberSequenceService;
        }

        [HttpGet]
        [Authorize(Policy = "NumberSequence.Index")]
        public async Task<IActionResult> Index()
        {
            var sequences = await _numberSequenceService.GetAllAsync();
            
            var vm = sequences.Select(s => new Clinic.Web.Models.NumberSequenceViewModel
            {
                Code = s.SequenceCode,
                CurrentValue = s.CurrentValue,
                ResetPolicy = s.ResetPolicy.ToString(),
                Prefix = s.Prefix,
                DatePattern = s.DatePattern,
                Padding = s.Padding
            }).ToList();

            foreach(var item in vm)
            {
                item.NextPreview = await _numberSequenceService.PreviewNextNumberAsync(item.Code);
            }

            return View(vm);
        }
    }
}
