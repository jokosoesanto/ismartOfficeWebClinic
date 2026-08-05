using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;

namespace Clinic.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var uiMetadata = new UIMetadata
            {
                ModuleName = "Settings",
                Title = "Personal Settings",
                Mode = RenderingMode.Template,
                Data = new { }
            };

            return View("Templates/Settings_Index", uiMetadata);
        }
    }
}
