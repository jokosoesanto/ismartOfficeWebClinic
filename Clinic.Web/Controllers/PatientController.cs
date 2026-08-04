using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class PatientController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var meta = new UIMetadata
            {
                Title = "Patient List",
                ModuleName = "Patient",
                Mode = RenderingMode.Template
            };

            var data = new List<ExpandoObject>();
            
            dynamic p1 = new ExpandoObject();
            p1.Id = "P001";
            p1.Name = "John Doe";
            p1.DOB = "01/01/1990";
            p1.Gender = "Male";
            p1.Phone = "555-0101";
            
            dynamic p2 = new ExpandoObject();
            p2.Id = "P002";
            p2.Name = "Jane Smith";
            p2.DOB = "15/05/1985";
            p2.Gender = "Female";
            p2.Phone = "555-0202";

            data.Add(p1);
            data.Add(p2);

            meta.Data = data;

            return View("Templates/Patient_List", meta);
        }

        [HttpGet("{id}")]
        public IActionResult Details(string id)
        {
            var meta = new UIMetadata
            {
                Title = $"Patient Details - {id}",
                ModuleName = "Patient",
                Mode = RenderingMode.Template
            };
            return View("Templates/Patient_Detail", meta);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Patient",
                ModuleName = "Patient",
                Mode = RenderingMode.Template
            };
            return View("Templates/Patient_Form", meta);
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(string id)
        {
            return View("Templates/Patient_Form", new UIMetadata { Title = "Edit Patient", ModuleName = "Patient", Mode = RenderingMode.Template });
        }
    }
}
