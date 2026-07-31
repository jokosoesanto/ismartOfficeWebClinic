using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    [Route("[controller]")]
    public class InventoryController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var metadata = new UIMetadata
            {
                Title = "Inventory",
                ModuleName = "Inventory",
                Mode = RenderingMode.Template
            };
            return View("Templates/Inventory_List", metadata);
        }

        [HttpGet("CreateItem")]
        public IActionResult CreateItem()
        {
            var metadata = new UIMetadata { Title = "Add Inventory Item", ModuleName = "Inventory", Mode = RenderingMode.Template };
            return View("Templates/Inventory_ItemForm", metadata);
        }

        [HttpGet("CreateGroup")]
        public IActionResult CreateGroup()
        {
            var metadata = new UIMetadata { Title = "Add Inventory Group", ModuleName = "Inventory", Mode = RenderingMode.Template };
            return View("Templates/Inventory_GroupForm", metadata);
        }

        [HttpGet("{id}")]
        public IActionResult Details(string id)
        {
            var metadata = new UIMetadata { Title = "Inventory Detail", ModuleName = "Inventory", Mode = RenderingMode.Template };
            return View("Templates/Inventory_Detail", metadata);
        }
    }
}
