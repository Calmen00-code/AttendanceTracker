using APIClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AttendanceTrackerApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceTrackerApplicationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private static readonly string _apiurl = "https://localhost:7102/api/AttendanceTrackerInfrastructure/";
        private static readonly string _deploymentapiurl = "https://attendancetrackerinfrastructure.azurewebsites.net/api/AttendanceTrackerInfrastructure/";

        public AttendanceTrackerApplicationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        [Route("get-admins")]
        public async Task<IActionResult> GetAdmins()
        {
            // string route = _apiurl + "get-admins";
            string route = _deploymentapiurl + "get-admins";
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
            // string route = _apiurl + "get-staffs";
            string route = _deploymentapiurl + "get-staffs";
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
            // string route = _apiurl + "get-staffs";
            string route = _deploymentapiurl + "get-staff/" + name;
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
            string route = _deploymentapiurl + "is-staff-exist/" + name;
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
            // string route = _apiurl + "get-admin/" + adminName;
            string route = _deploymentapiurl + "get-admin/" + adminName;
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
            // string route = _apiurl + "get-departments";
            string route = _deploymentapiurl + "get-departments";
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

            // string route = _apiurl + "add-admin";
            string route = _deploymentapiurl + "add-admin";
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

            // string route = _apiurl + "add-staff";
            string route = _deploymentapiurl + "add-staff";
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

            // string route = _apiurl + "add-workday-record";
            string route = _deploymentapiurl + "add-workday-record";
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

            // string route = _apiurl + "add-department";
            string route = _deploymentapiurl + "add-department";
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
        [Route("checkin/{Username}/{CurrentTime}")]
        public async Task<IActionResult> TimesheetCheckIn(string Username, string CurrentTime)
        {
            var response = (HttpResponseMessage)(await IsStaffExist(Username));
            
            // only proceed if staff is valid
            if ((int) response.StatusCode == HttpResponseStatus.OK)
            {
                // Constructing staff object for workday
                var response_staff = (HttpResponseMessage)(await GetStaff(Username));
                if ((int) response_staff.StatusCode == HttpResponseStatus.OK)
                {
                    StaffAPI staff = await response_staff.Content.ReadFromJsonAsync<StaffAPI>();

                    if (staff == null)
                    {
                        return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Server return bad data on staff");
                    }
                    else
                    {
                        DateTime alignedDateTime = DateTime.Parse(AlignDateFormat(CurrentTime));

                        /**
                         * if alignedDateTime does not have today's date, it means the user just check in for the day. 
                         * Use POST to create new date
                         * 
                         * if alignedDateTime already has an existed date, this mean the user was checking out and 
                         * checking in back, use PUT to update the date instead
                         */
                        bool shouldUsePost = true;
                        foreach (WorkdayRecordAPI workday in staff.Workdays)
                        {
                            if (workday.Date == alignedDateTime)
                            {
                                shouldUsePost = false;
                                break;
                            }
                        }

                        WorkdayRecordAPI staffWorkday = new WorkdayRecordAPI();
                        staffWorkday.Date = alignedDateTime;
                        staffWorkday.CheckIn = alignedDateTime;
                        staffWorkday.StaffName = Username;
                        staffWorkday.Staff = staff;

                        var workdayJson = JsonContent.Create(staffWorkday);
                        if (shouldUsePost)
                        {
                            string route_post = _deploymentapiurl + "add-workday-record";
                            var response_post = await _httpClient.PostAsync(route_post, workdayJson);

                            if ((int)(response.StatusCode) == HttpResponseStatus.CREATED)
                            {
                                return StatusCode(HttpResponseStatus.CREATED);
                            }
                            else
                            {
                                string errorContent = await response.Content.ReadAsStringAsync();
                                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, errorContent);
                            }
                        }
                        else
                        {
                            string route_update = _deploymentapiurl + "update-workday";
                            var response_update = await _httpClient.PutAsync(route_update, workdayJson);

                            if ((int)(response.StatusCode) == HttpResponseStatus.ACCEPTED)
                            {
                                return StatusCode(HttpResponseStatus.ACCEPTED);
                            }
                            else
                            {
                                string errorContent = await response.Content.ReadAsStringAsync();
                                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, errorContent);
                            }
                        }
                    }
                }
                else
                {
                    return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Could not load user");
                }
            }
            else
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "User does not exist!");
            }
        }

        [HttpPost]
        [Route("checkout/{Username}/{CurrentTime}")]
        public async Task<IActionResult> TimesheetCheckOut(string Username, string CurrentTime)
        {
            var response = (HttpResponseMessage)(await IsStaffExist(Username));
            
            // only proceed if staff is valid
            if ((int) response.StatusCode == HttpResponseStatus.OK)
            {
                // Constructing staff object for workday
                var response_staff = (HttpResponseMessage)(await GetStaff(Username));
                if ((int) response_staff.StatusCode == HttpResponseStatus.OK)
                {
                    StaffAPI staff = await response_staff.Content.ReadFromJsonAsync<StaffAPI>();

                    if (staff == null)
                    {
                        return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Server return bad data on staff");
                    }
                    else
                    {
                        DateTime alignedDateTime = DateTime.Parse(AlignDateFormat(CurrentTime));

                        /**
                         * if alignedDateTime does not have existing check in date, abort the operation
                         */
                        bool checkInExist = false;
                        foreach (WorkdayRecordAPI workday in staff.Workdays)
                        {
                            if (workday.Date == alignedDateTime)
                            {
                                checkInExist = true;
                                break;
                            }
                        }

                        if (checkInExist)
                        {
                            WorkdayRecordAPI staffWorkday = new WorkdayRecordAPI();
                            staffWorkday.Date = alignedDateTime;
                            staffWorkday.CheckOut = alignedDateTime;
                            staffWorkday.StaffName = Username;
                            staffWorkday.Staff = staff;

                            var workdayJson = JsonContent.Create(staffWorkday);
                            string route_update = _deploymentapiurl + "update-workday";
                            var response_update = await _httpClient.PutAsync(route_update, workdayJson);

                            if ((int)(response.StatusCode) == HttpResponseStatus.ACCEPTED)
                            {
                                return StatusCode(HttpResponseStatus.ACCEPTED);
                            }
                            else
                            {
                                string errorContent = await response.Content.ReadAsStringAsync();
                                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, errorContent);
                            }
                        }
                        else
                        {
                            return StatusCode(HttpResponseStatus.NOT_FOUND, "Check-Out aborted: Because no Check-In time has been found");
                        }
                    }
                }
                else
                {
                    return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Could not load user");
                }
            }
            else
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "User does not exist!");
            }
        }

        /*
        [HttpPut]
        [Route("update-workday")]
        public async Task<IActionResult> UpdateWorkday([FromBody] WorkdayRecordAPI workdayRecordAPI)
        {

            // string route = _apiurl + "add-department";
            string route = _deploymentapiurl + "update-workday";

            // System.Console.WriteLine("StaffName: " + workdayRecordAPI.StaffName);
            // System.Console.WriteLine("CheckIn: " + workdayRecordAPI.CheckIn);
            // System.Console.WriteLine("CheckOut: " + workdayRecordAPI.CheckOut);
            // System.Console.WriteLine("Date: " + workdayRecordAPI.Date);

            var response = await _httpClient.PutAsync(route, workdayJson);

            if ((int)(response.StatusCode) == HttpResponseStatus.ACCEPTED)
            {
                return StatusCode(HttpResponseStatus.ACCEPTED);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, errorContent);
            }
        }
        */

        // PRIVATE FUNCTION

        // function used to align the dateTime to compatible with Database format
        private string AlignDateFormat(string dateTime)
        {
            return DateTime.Parse(dateTime).ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }
}
