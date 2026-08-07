using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.Interfaces.Storage;
using Clinic.Application.Interfaces.Auth;
using Clinic.Application.DTOs.MasterData;
using Clinic.Domain.Entities.MasterData;
using Clinic.Domain.Entities.System;
using Clinic.Application.UI;
using Clinic.Infrastructure.Data;
using Clinic.Web.Extensions;
using Clinic.Web.Models.Patient;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IMasterReferenceService _masterReferenceService;
        private readonly ILocationService _locationService;
        private readonly IFileStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppDbContext _context;

        public PatientController(
            IPatientService patientService,
            IMasterReferenceService masterReferenceService,
            ILocationService locationService,
            IFileStorageService storageService,
            ICurrentUserService currentUserService,
            AppDbContext context)
        {
            _patientService = patientService;
            _masterReferenceService = masterReferenceService;
            _locationService = locationService;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "Patient.View")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Patient List",
                ModuleName = "Patient",
                Mode = RenderingMode.Template
            };

            var patients = await _patientService.GetAllAsync();
            ViewBag.Meta = meta;
            return View(patients);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "Patient.Create")]
        public async Task<IActionResult> Create()
        {
            var model = await BuildFormViewModelAsync(new PatientDto());
            var meta = new UIMetadata
            {
                Title = "Register New Patient",
                ModuleName = "Patient",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            return View(model);
        }

        [HttpPost("Create")]
        [Authorize(Policy = "Patient.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientDto dto, Microsoft.AspNetCore.Http.IFormFile? PhotoUpload, bool ignoreDuplicate = false)
        {
            if (ModelState.IsValid)
            {
                if (!ignoreDuplicate)
                {
                    bool isDuplicate = await _patientService.IsDuplicateCandidateAsync(
                        dto.NationalId, dto.Mobile, dto.FullName, dto.BirthDate);

                    if (isDuplicate)
                    {
                        ModelState.AddModelError(string.Empty, "Possible Duplicate Patient found. Verify the data or click Save again to ignore.");
                        ViewBag.IgnoreDuplicate = true;
                        var duplicateModel = await BuildFormViewModelAsync(dto);
                        ViewBag.Meta = new UIMetadata { Title = "Register New Patient", ModuleName = "Patient", Mode = RenderingMode.Template };
                        return View(duplicateModel);
                    }
                }

                Guid? photoId = null;
                if (PhotoUpload != null && PhotoUpload.Length > 0)
                {
                    using var stream = PhotoUpload.OpenReadStream();
                    var fileMeta = await _storageService.UploadAsync(
                        stream,
                        PhotoUpload.FileName,
                        PhotoUpload.ContentType,
                        Clinic.Domain.Enums.StorageModule.Patient,
                        null,
                        _currentUserService.UserId ?? Guid.Empty);
                    
                    photoId = fileMeta.Id;
                }

                var patient = MapToEntity(dto);
                patient.PhotoFileMetadataId = photoId;

                await _patientService.CreateAsync(patient, _currentUserService.UserId ?? Guid.Empty);
                
                TempData["SuccessMessage"] = "Patient registered successfully. MRN: " + patient.MRN;
                return RedirectToAction(nameof(Index));
            }

            var model = await BuildFormViewModelAsync(dto);
            ViewBag.Meta = new UIMetadata { Title = "Register New Patient", ModuleName = "Patient", Mode = RenderingMode.Template };
            return View(model);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "Patient.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return NotFound();

            var dto = MapToDto(patient);
            var model = await BuildFormViewModelAsync(dto);
            
            var meta = new UIMetadata
            {
                Title = "Edit Patient",
                ModuleName = "Patient",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            return View(model);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "Patient.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PatientDto dto, Microsoft.AspNetCore.Http.IFormFile? PhotoUpload)
        {
            if (id != dto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var existing = await _patientService.GetByIdAsync(id);
                if (existing == null) return NotFound();

                Guid? photoId = existing.PhotoFileMetadataId;
                if (PhotoUpload != null && PhotoUpload.Length > 0)
                {
                    using var stream = PhotoUpload.OpenReadStream();
                    var fileMeta = await _storageService.UploadAsync(
                        stream,
                        PhotoUpload.FileName,
                        PhotoUpload.ContentType,
                        Clinic.Domain.Enums.StorageModule.Patient,
                        existing.Id,
                        _currentUserService.UserId ?? Guid.Empty);
                    
                    photoId = fileMeta.Id;
                }

                // ENTERPRISE AGGREGATE UPDATE STANDARD: 
                // Map properties to the tracked entity instead of creating a new Patient
                existing.FullName = dto.FullName;
                existing.NationalId = dto.NationalId;
                existing.PassportNumber = dto.PassportNumber;
                existing.Gender = dto.Gender;
                existing.BirthDate = dto.BirthDate;
                existing.BloodType = dto.BloodType;
                existing.Religion = dto.Religion;
                existing.Nationality = dto.Nationality;
                existing.Language = dto.Language;
                existing.Occupation = dto.Occupation;
                existing.Education = dto.Education;
                existing.MaritalStatus = dto.MaritalStatus;
                existing.Category = dto.Category;
                existing.Status = dto.Status;
                existing.Address = dto.Address;
                existing.Province = dto.Province;
                existing.City = dto.City;
                existing.Country = dto.Country;
                existing.PostalCode = dto.PostalCode;
                existing.Email = dto.Email;
                existing.Mobile = dto.Mobile;
                existing.WhatsApp = dto.WhatsApp;
                existing.HomePhone = dto.HomePhone;
                existing.WorkPhone = dto.WorkPhone;
                existing.EmergencyContactName = dto.EmergencyContactName;
                existing.EmergencyRelationship = dto.EmergencyRelationship;
                existing.EmergencyPhone = dto.EmergencyPhone;
                existing.EmergencyAddress = dto.EmergencyAddress;
                existing.PreferredCommunication = dto.PreferredCommunication;
                existing.HomeClinicId = dto.HomeClinicId;
                existing.RegistrationDate = dto.RegistrationDate;
                existing.Notes = dto.Notes;
                existing.PhotoFileMetadataId = photoId;

                await _patientService.UpdateAsync(existing, _currentUserService.UserId ?? Guid.Empty);
                
                TempData["SuccessMessage"] = "Patient updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            var model = await BuildFormViewModelAsync(dto);
            ViewBag.Meta = new UIMetadata { Title = "Edit Patient", ModuleName = "Patient", Mode = RenderingMode.Template };
            return View(model);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Policy = "Patient.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _patientService.DeleteAsync(id, _currentUserService.UserId ?? Guid.Empty);
            TempData["SuccessMessage"] = "Patient deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Search")]
        [Authorize(Policy = "Patient.View")]
        public async Task<IActionResult> Search(string? mrn, string? nationalId, string? name, string? phone)
        {
            var patients = await _patientService.SearchAsync(mrn, nationalId, null, name, phone, null, null);
            return Json(patients.Select(p => new {
                p.Id,
                p.MRN,
                p.FullName,
                p.Mobile,
                p.BirthDate
            }));
        }

        [HttpGet("Photo/{id}")]
        [AllowAnonymous] // Assuming token in query string or we rely on session
        public async Task<IActionResult> Photo(Guid id)
        {
            try
            {
                var fileMeta = await _context.FileMetadatas.FindAsync(id);
                if (fileMeta == null) return NotFound();

                var stream = await _storageService.OpenReadAsync(fileMeta);
                if (stream == null) return NotFound();
                
                return File(stream, fileMeta.MimeType ?? "application/octet-stream");
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("Thumbnail/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Thumbnail(Guid id)
        {
            try
            {
                var fileMeta = await _context.FileMetadatas.FindAsync(id);
                if (fileMeta == null) return NotFound();

                var stream = await _storageService.OpenThumbnailAsync(fileMeta);
                if (stream == null) return NotFound();
                
                return File(stream, fileMeta.MimeType ?? "application/octet-stream");
            }
            catch
            {
                return NotFound();
            }
        }

        private async Task<PatientFormViewModel> BuildFormViewModelAsync(PatientDto dto)
        {
            var model = new PatientFormViewModel { Patient = dto ?? new PatientDto() };
            
            var locations = await _locationService.GetAllLocationsAsync();
            model.Locations = locations.Where(l => l.IsActive).ToSelectList(x => x.Id, x => x.ClinicName, dto?.HomeClinicId);

            model.Genders = (await _masterReferenceService.GetByCategoryAsync("Gender")).ToSelectList(x => x.Code, x => x.Name, dto?.Gender);
            model.BloodTypes = (await _masterReferenceService.GetByCategoryAsync("BloodType")).ToSelectList(x => x.Code, x => x.Name, dto?.BloodType);
            model.Religions = (await _masterReferenceService.GetByCategoryAsync("Religion")).ToSelectList(x => x.Code, x => x.Name, dto?.Religion);
            model.Nationalities = (await _masterReferenceService.GetByCategoryAsync("Nationality")).ToSelectList(x => x.Code, x => x.Name, dto?.Nationality);
            model.Languages = (await _masterReferenceService.GetByCategoryAsync("Language")).ToSelectList(x => x.Code, x => x.Name, dto?.Language);
            model.Occupations = (await _masterReferenceService.GetByCategoryAsync("Occupation")).ToSelectList(x => x.Code, x => x.Name, dto?.Occupation);
            model.Educations = (await _masterReferenceService.GetByCategoryAsync("Education")).ToSelectList(x => x.Code, x => x.Name, dto?.Education);
            model.MaritalStatuses = (await _masterReferenceService.GetByCategoryAsync("MaritalStatus")).ToSelectList(x => x.Code, x => x.Name, dto?.MaritalStatus);
            model.Categories = (await _masterReferenceService.GetByCategoryAsync("PatientCategory")).ToSelectList(x => x.Code, x => x.Name, dto?.Category);
            model.Statuses = (await _masterReferenceService.GetByCategoryAsync("PatientStatus")).ToSelectList(x => x.Code, x => x.Name, dto?.Status);
            model.Relationships = (await _masterReferenceService.GetByCategoryAsync("Relationship")).ToSelectList(x => x.Code, x => x.Name, dto?.EmergencyRelationship);
            model.Countries = (await _masterReferenceService.GetByCategoryAsync("Country")).ToSelectList(x => x.Code, x => x.Name, dto?.Country);
            model.Provinces = (await _masterReferenceService.GetByCategoryAsync("Province")).ToSelectList(x => x.Code, x => x.Name, dto?.Province);
            model.PreferredCommunications = (await _masterReferenceService.GetByCategoryAsync("PreferredCommunication")).ToSelectList(x => x.Code, x => x.Name, dto?.PreferredCommunication);

            return model;
        }

        private Patient MapToEntity(PatientDto dto)
        {
            return new Patient
            {
                Id = dto.Id ?? Guid.NewGuid(),
                FullName = dto.FullName,
                NationalId = dto.NationalId,
                PassportNumber = dto.PassportNumber,
                Gender = dto.Gender,
                BirthDate = dto.BirthDate,
                BloodType = dto.BloodType,
                Religion = dto.Religion,
                Nationality = dto.Nationality,
                Language = dto.Language,
                Occupation = dto.Occupation,
                Education = dto.Education,
                MaritalStatus = dto.MaritalStatus,
                Category = dto.Category,
                Status = dto.Status,
                Address = dto.Address,
                Province = dto.Province,
                City = dto.City,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Email = dto.Email,
                Mobile = dto.Mobile,
                WhatsApp = dto.WhatsApp,
                HomePhone = dto.HomePhone,
                WorkPhone = dto.WorkPhone,
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyRelationship = dto.EmergencyRelationship,
                EmergencyPhone = dto.EmergencyPhone,
                EmergencyAddress = dto.EmergencyAddress,
                PreferredCommunication = dto.PreferredCommunication,
                HomeClinicId = dto.HomeClinicId,
                RegistrationDate = dto.RegistrationDate,
                Notes = dto.Notes
            };
        }

        private PatientDto MapToDto(Patient p)
        {
            return new PatientDto
            {
                Id = p.Id,
                MRN = p.MRN,
                FullName = p.FullName,
                NationalId = p.NationalId,
                PassportNumber = p.PassportNumber,
                Gender = p.Gender,
                BirthDate = p.BirthDate,
                BloodType = p.BloodType,
                Religion = p.Religion,
                Nationality = p.Nationality,
                Language = p.Language,
                Occupation = p.Occupation,
                Education = p.Education,
                MaritalStatus = p.MaritalStatus,
                Category = p.Category,
                Status = p.Status,
                PhotoFileMetadataId = p.PhotoFileMetadataId,
                Address = p.Address,
                Province = p.Province,
                City = p.City,
                Country = p.Country,
                PostalCode = p.PostalCode,
                Email = p.Email,
                Mobile = p.Mobile,
                WhatsApp = p.WhatsApp,
                HomePhone = p.HomePhone,
                WorkPhone = p.WorkPhone,
                EmergencyContactName = p.EmergencyContactName,
                EmergencyRelationship = p.EmergencyRelationship,
                EmergencyPhone = p.EmergencyPhone,
                EmergencyAddress = p.EmergencyAddress,
                PreferredCommunication = p.PreferredCommunication,
                HomeClinicId = p.HomeClinicId,
                RegistrationDate = p.RegistrationDate,
                Notes = p.Notes
            };
        }
    }
}
