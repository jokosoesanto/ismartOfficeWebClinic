using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Clinic.Application.Interfaces.Operations;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Web.Extensions;

namespace Clinic.Web.Controllers
{
    [Authorize]
    public class ScheduleBoardController : Controller
    {
        private readonly IScheduleBoardRepository _scheduleBoardRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ISpecialtyRepository _specialtyRepository;

        public ScheduleBoardController(
            IScheduleBoardRepository scheduleBoardRepository,
            ILocationRepository locationRepository,
            ISpecialtyRepository specialtyRepository)
        {
            _scheduleBoardRepository = scheduleBoardRepository;
            _locationRepository = locationRepository;
            _specialtyRepository = specialtyRepository;
        }

        [Authorize(Policy = "ScheduleBoard.View")]
        public async Task<IActionResult> Index()
        {
            var locations = await _locationRepository.GetAllAsync();
            var specialties = await _specialtyRepository.GetAllAsync();

            ViewBag.Locations = locations.ToSelectList(x => x.Id, x => x.ClinicName);
            ViewBag.Specialties = specialties.ToSelectList(x => x.Id, x => x.Name);

            return View();
        }

        [Authorize(Policy = "ScheduleBoard.View")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                // Custom filters
                var locationIdStr = Request.Form["locationId"].FirstOrDefault();
                var specialtyIdStr = Request.Form["specialtyId"].FirstOrDefault();
                var dateStr = Request.Form["date"].FirstOrDefault();
                var availabilityStr = Request.Form["availability"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                Guid? locationId = string.IsNullOrEmpty(locationIdStr) ? null : Guid.Parse(locationIdStr);
                Guid? specialtyId = string.IsNullOrEmpty(specialtyIdStr) ? null : Guid.Parse(specialtyIdStr);
                DateTime? date = string.IsNullOrEmpty(dateStr) ? null : DateTime.Parse(dateStr);
                
                var data = await _scheduleBoardRepository.GetSchedulesAsync(
                    locationId, null, specialtyId, null, date, searchValue);

                // Filter Availability client-side to keep repo clean of presentation logic
                if (!string.IsNullOrEmpty(availabilityStr))
                {
                    data = data.Where(x => x.Status.Equals(availabilityStr, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    data = sortColumnDirection == "asc" 
                        ? data.OrderBy(x => GetPropertyValue(x, sortColumn)).ToList()
                        : data.OrderByDescending(x => GetPropertyValue(x, sortColumn)).ToList();
                }

                recordsTotal = data.Count();
                var recordsFiltered = recordsTotal;

                if (pageSize > 0)
                {
                    data = data.Skip(skip).Take(pageSize).ToList();
                }

                return Json(new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (prop == null) return null!;
            return prop.GetValue(obj, null)!;
        }

        [Authorize(Policy = "ScheduleBoard.View")]
        [HttpGet]
        public async Task<IActionResult> GetCalendarData(DateTime start, DateTime end, Guid? locationId, Guid? specialtyId, string? availability)
        {
            var events = new System.Collections.Generic.List<object>();
            
            // Loop through each day in the visible range
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            {
                var schedules = await _scheduleBoardRepository.GetSchedulesAsync(locationId, null, specialtyId, null, d, null);
                
                if (!string.IsNullOrEmpty(availability))
                {
                    schedules = schedules.Where(x => x.Status.Equals(availability, StringComparison.OrdinalIgnoreCase));
                }

                foreach (var s in schedules)
                {
                    // Create FullCalendar event
                    if (!s.StartTime.HasValue || !s.EndTime.HasValue)
                        continue;

                    var startTimeSpan = s.StartTime.Value;
                    var endTimeSpan = s.EndTime.Value;

                    var eventStart = d.Add(startTimeSpan).ToString("yyyy-MM-ddTHH:mm:ss");
                    var eventEnd = d.Add(endTimeSpan).ToString("yyyy-MM-ddTHH:mm:ss");

                    events.Add(new
                    {
                        doctorName = s.DoctorName,
                        eventStart = eventStart,
                        eventEnd = eventEnd,
                        color = string.IsNullOrEmpty(s.DoctorColor) ? "#0d6efd" : s.DoctorColor,
                        specialty = s.Specialty,
                        locationName = s.LocationName,
                        chair = s.Chair,
                        status = s.Status
                    });
                }
            }

            return Json(events);
        }
    }
}
