using APIClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Data.SqlTypes;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace AttendanceTrackerApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceTrackerApplicationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private static readonly string _apiurl = APILinks.API_URL_INFRASTRUCTURE_DEVELOPMENT;
        // private static readonly string _apiurl = APILinks.API_URL_INFRASTRUCTURE_DEPLOYMENT;

        public AttendanceTrackerApplicationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        [Route("get-admins")]
        public async Task<IActionResult> GetAdmins()
        {
            string route = _apiurl + "get-admins";
            var response = await _httpClient.GetAsync(route);

            if ((int)(response.StatusCode) == HttpResponseStatus.OK)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.OK, responseContent);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpGet]
        [Route("get-staffs")]
        public async Task<IActionResult> GetStaffs()
        {
            string route = _apiurl + "get-staffs";
            var response = await _httpClient.GetAsync(route);

            if ((int)(response.StatusCode) == HttpResponseStatus.OK)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.OK, responseContent);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpGet]
        [Route("get-staff/{name}")]
        public async Task<IActionResult> GetStaff(string name)
        {
            string route = _apiurl + "get-staffs";
            var response = await _httpClient.GetAsync(route);

            if ((int)(response.StatusCode) == HttpResponseStatus.OK)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.OK, responseContent);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpGet]
        [Route("is-staff-exist/{name}")]
        public async Task<IActionResult> IsStaffExist(string name)
        {
            string route = _apiurl + "is-staff-exist/" + name;
            var response = await _httpClient.GetAsync(route);

            if ((int)(response.StatusCode) == HttpResponseStatus.OK)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.OK, responseContent);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpGet]
        [Route("get-admin/{adminName}")]
        public async Task<IActionResult> GetAdmin(string adminName)
        {
            string route = _apiurl + "get-admin/" + adminName;
            var response = await _httpClient.GetAsync(route);

            if ((int)(response.StatusCode) == HttpResponseStatus.OK) 
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.OK, responseContent);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpGet]
        [Route("get-departments")]
        public async Task<IActionResult> GetDepartments()
        {
            string route = _apiurl + "get-departments";
            var response = await _httpClient.GetAsync(route);

            if ((int)(response.StatusCode) == HttpResponseStatus.OK)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.OK, responseContent);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpPost]
        [Route("add-admin")]
        public async Task<IActionResult> AddAdmin([FromBody] AdminAPI admin) 
        {
            // Serializing individual data of admin and used it to create
            // a json object representation for it
            var adminJson = JsonContent.Create(admin);

            string route = _apiurl + "add-admin";
            var response = await _httpClient.PostAsync(route, adminJson);

            if ((int)(response.StatusCode) == HttpResponseStatus.CREATED)
            {
                /*
                var adminContent = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await JsonSerializer.DeserializeAsync<AdminAPI>(adminContent, options);
                */

                return StatusCode(HttpResponseStatus.CREATED);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpPost]
        [Route("add-staff")]
        public async Task<IActionResult> AddStaff([FromBody] StaffAPI staff)
        {
            var staffJson = JsonContent.Create(staff);
            System.Diagnostics.Debug.WriteLine("Department in Applcaitioon:" + staff.Department);

            string route = _apiurl + "add-staff";
            var response = await _httpClient.PostAsync(route, staffJson);

            if ((int)(response.StatusCode) == HttpResponseStatus.CREATED)
            {
                // var staffContent = await response.Content.ReadAsStreamAsync();
                // var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // var result = await JsonSerializer.DeserializeAsync<StaffAPI>(staffContent, options);

                // return StatusCode(HttpResponseStatus.CREATED, result);
                return StatusCode(HttpResponseStatus.CREATED);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpPost]
        [Route("add-workday-record")]
        public async Task<IActionResult> AddWorkdayRecord([FromBody] WorkdayRecordAPI workdayRecord)
        {
            // aligned the workday record with the format in the database
            // TODO 
            // WorkdayRecordAPI workdayRecordDbFormat = new WorkdayRecordAPI();
            // DateTime CheckIn = DateTime.ParseExact()

            var workdayRecordJson = JsonContent.Create(workdayRecord);

            string route = _apiurl + "add-workday-record";
            var response = await _httpClient.PostAsync(route, workdayRecordJson);

            if ((int)(response.StatusCode) == HttpResponseStatus.CREATED)
            {
                /*
                var workdayContent = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await JsonSerializer.DeserializeAsync<WorkdayRecordAPI>(workdayContent, options);
                */

                return StatusCode(HttpResponseStatus.CREATED);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpPost]
        [Route("add-department")]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentAPI departmentAPI)
        {
            var departmentJson = JsonContent.Create(departmentAPI);

            string route = _apiurl + "add-department";
            var response = await _httpClient.PostAsync(route, departmentJson);

            if ((int)(response.StatusCode) == HttpResponseStatus.CREATED)
            {
                /*
                var workdayContent = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await JsonSerializer.DeserializeAsync<WorkdayRecordAPI>(workdayContent, options);
                */

                return StatusCode(HttpResponseStatus.CREATED);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }

        [HttpPost]
        [Route("checkinn")]
        public async Task<IActionResult> TimesheetCheckInn([FromBody] TimesheetAPI timesheet)
        {
            System.Diagnostics.Debug.WriteLine("Test");
            return StatusCode(HttpResponseStatus.NOT_FOUND, "DEBUG");
        }


        [HttpPost]
        [Route("checkin")]
        public async Task<IActionResult> TimesheetCheckIn([FromBody] TimesheetAPI timesheet)
        {
            // Note: we are calling API function directly in this file, instead of through Http network
            // var response = (ObjectResult)(await IsStaffExist(timesheet.Username));
            string route_staff_exist = _apiurl + "is-staff-exist/" + timesheet.Username;
            var response_staff_exist = await _httpClient.GetAsync(route_staff_exist);

            if ((int)response_staff_exist.StatusCode != HttpResponseStatus.OK)
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "User does not exist!");
            }

            // Constructing staff object for workday
            // var response_staff = (ObjectResult)(await GetStaff(timesheet.Username));
            string route_staff = _apiurl + "get-staff/" + timesheet.Username;
            var response_staff = await _httpClient.GetAsync(route_staff);
            if ((int)response_staff.StatusCode != HttpResponseStatus.OK)
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Could not load user");
            }

            // Converting ObjectResult response_staff to StaffAPI because ObjectResult is upcast,
            // we cannot downcast from ObjectResult so we have to deserialize as such
            // StaffAPI staff = JsonConvert.DeserializeObject<StaffAPI>((string)response_staff.Value);
            StaffAPI staff = await response_staff.Content.ReadFromJsonAsync<StaffAPI>();

            // only proceed if staff is valid
            if (staff == null)
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Server return bad data on staff");
            }

            string route_query_workdays = _apiurl + "get-workdays";
            var response_query_workdays = await _httpClient.GetAsync(route_query_workdays);
            if ((int)(response_query_workdays.StatusCode) != HttpResponseStatus.OK)
            {
                string errorContent = await response_query_workdays.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, errorContent);
            }

            /**
             * if alignedDateTime does not have today's date, it means the user just check in for the day. 
             * Use POST to create new date
             * 
             * if alignedDateTime already has an existed date, this mean the user was checking out and 
             * checking in back, use PUT to update the date instead
             */
            List<WorkdayRecordAPI> workdays = await response_query_workdays.Content.ReadFromJsonAsync<List<WorkdayRecordAPI>>();
            DateTime alignedCheckInTime = DateTime.Parse(AlignDateFormat(timesheet.CurrentTime));
            bool shouldUsePost = true;
            foreach (WorkdayRecordAPI workday in workdays)
            {
                // we only compare against the Date using ToShortDateString(), excluding the time, only date
                if (workday.StaffName.Equals(timesheet.Username) && IsDateSame(alignedCheckInTime, workday.Date))
                {
                    shouldUsePost = false;
                    break;
                }
            }

            WorkdayRecordAPI staffWorkday = new WorkdayRecordAPI();
            staffWorkday.Date = alignedCheckInTime;
            staffWorkday.CheckIn = alignedCheckInTime;
            staffWorkday.CheckOut = (DateTime)SqlDateTime.MinValue;
            staffWorkday.StaffName = timesheet.Username;
            staffWorkday.TotalWorkingHours = 0;
            staffWorkday.Staff = staff;

            var workdayJson = JsonContent.Create(staffWorkday);

            if (shouldUsePost)
            {
                string route_post = _apiurl + "add-workday-record";
                var jsonString = await workdayJson.ReadAsStringAsync();

                var response_post = await _httpClient.PostAsync(route_post, workdayJson);

                if ((int)(response_post.StatusCode) == HttpResponseStatus.CREATED)
                {
                    return StatusCode(HttpResponseStatus.CREATED);
                }
                else 
                {
                    return StatusCode(HttpResponseStatus.OK, "add workday record failed");
                }
            }
            else
            {
                string route_update = _apiurl + "update-workday";
                var response_update = await _httpClient.PutAsJsonAsync(route_update, staffWorkday);

                if ((int)(response_update.StatusCode) == HttpResponseStatus.ACCEPTED)
                {
                    return StatusCode(HttpResponseStatus.ACCEPTED);
                }
                else
                {
                    string errorContent = await response_update.Content.ReadAsStringAsync();
                    return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, errorContent);
                }
            }
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> TimesheetCheckOut([FromBody] TimesheetAPI timesheet)
        {
            // Note: we are calling API function directly in this file, instead of through Http network
            // var response = (ObjectResult)(await IsStaffExist(timesheet.Username));
            string route_staff_exist = _apiurl + "is-staff-exist/" + timesheet.Username;
            var response_staff_exist = await _httpClient.GetAsync(route_staff_exist);

            if ((int)response_staff_exist.StatusCode != HttpResponseStatus.OK)
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "User does not exist!");
            }

            // Constructing staff object for workday
            // var response_staff = (ObjectResult)(await GetStaff(timesheet.Username));
            string route_staff = _apiurl + "get-staff/" + timesheet.Username;
            var response_staff = await _httpClient.GetAsync(route_staff);
            if ((int)response_staff.StatusCode != HttpResponseStatus.OK)
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Could not load user");
            }

            // Converting ObjectResult response_staff to StaffAPI because ObjectResult is upcast,
            // we cannot downcast from ObjectResult so we have to deserialize as such
            // StaffAPI staff = JsonConvert.DeserializeObject<StaffAPI>((string)response_staff.Value);
            StaffAPI staff = await response_staff.Content.ReadFromJsonAsync<StaffAPI>();

            // only proceed if staff is valid
            if (staff == null)
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Server return bad data on staff");
            }

            // Getting workdays information to determine whether checkout should proceed or abort
            string route_query_workdays = _apiurl + "get-workdays";
            var response_query_workdays = await _httpClient.GetAsync(route_query_workdays);
            if ((int)(response_query_workdays.StatusCode) != HttpResponseStatus.OK)
            {
                string errorContent = await response_query_workdays.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, errorContent);
            }

            List<WorkdayRecordAPI> workdays = await response_query_workdays.Content.ReadFromJsonAsync<List<WorkdayRecordAPI>>();
            DateTime alignedCheckOutTime = DateTime.Parse(AlignDateFormat(timesheet.CurrentTime));
            WorkdayRecordAPI? userWorkday = null;
            foreach (WorkdayRecordAPI workday in workdays)
            {
                // we only compare against the Date using ToShortDateString(), excluding the time, only date
                if (workday.StaffName.Equals(timesheet.Username) &&
                    workday.Date.ToShortDateString().Equals(alignedCheckOutTime.ToShortDateString()))
                {
                    userWorkday = workday;
                    break;
                }
            }

            // if userWorkday is null, this means that user has never checked in before, abort
            // as user cannot check out without checking in first
            if (userWorkday == null)
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "Please check in first!");
            }

            WorkdayRecordAPI staffWorkday = new WorkdayRecordAPI();
            staffWorkday.Date = alignedCheckOutTime;
            staffWorkday.CheckIn = userWorkday.CheckIn;
            staffWorkday.CheckOut = alignedCheckOutTime;
            staffWorkday.StaffName = timesheet.Username;
            staffWorkday.Staff = staff;

            // Getting the elapsed time for TotalWorkingHours
            TimeSpan elapsedTime = alignedCheckOutTime.Subtract(userWorkday.CheckIn);
            staffWorkday.TotalWorkingHours = (decimal)(elapsedTime.TotalHours);

            var workdayJson = JsonContent.Create(staffWorkday);

            string route_update = _apiurl + "update-workday";

            // string jsonString = await workdayJson.ReadAsStringAsync();
            // return StatusCode(HttpResponseStatus.OK, jsonString);

            var response_update = await _httpClient.PutAsJsonAsync(route_update, staffWorkday);

            if ((int)(response_update.StatusCode) == HttpResponseStatus.ACCEPTED)
            {
                return StatusCode(HttpResponseStatus.ACCEPTED);
            }
            else
            {
                string errorContent = await response_update.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, errorContent);
            }
        }

        // PRIVATE FUNCTION

        // function used to align the dateTime to compatible with Database format
        private string AlignDateFormat(string dateTime)
        {
            return DateTime.Parse(dateTime).ToString("yyyy-MM-ddTHH:mm:ss");
        }

        private bool IsDateSame(DateTime date1, DateTime date2)
        {
            return (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day);
        }
    }
}
