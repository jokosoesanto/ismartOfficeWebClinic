using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.UI;
using System.Collections.Generic;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class BillingController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var metadata = new UIMetadata
            {
                Title = "Billing Dashboard",
                ModuleName = "Billing",
                Mode = RenderingMode.Template
            };
            return View("Templates/TransactionList", metadata);
        }

        [HttpGet("Payment")]
        public IActionResult Payment()
        {
            var metadata = new UIMetadata { Title = "Process Payment", ModuleName = "Billing", Mode = RenderingMode.Template };
            return View("Templates/Payment_Form", metadata);
        }

        [HttpGet("History")]
        public IActionResult History()
        {
            var metadata = new UIMetadata { Title = "Payment History", ModuleName = "Billing", Mode = RenderingMode.Template };
            return View("Templates/Payment_History", metadata);
        }

        [HttpGet("Preview/{id}")]
        public IActionResult Preview(string id)
        {
            var metadata = new UIMetadata { Title = "Receipt Preview", ModuleName = "Billing", Mode = RenderingMode.Template };
            return View("Templates/Payment_Preview", metadata);
        }
    }
}
