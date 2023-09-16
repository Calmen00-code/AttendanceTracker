using APIClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
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
            var workdayRecordJson = JsonContent.Create(workdayRecord);

            string route = _apiurl + "add-workday-record";
            var response = await _httpClient.PostAsync(route, workdayRecordJson);

            if ((int)(response.StatusCode) == HttpResponseStatus.ACCEPTED)
            {
                /*
                var workdayContent = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await JsonSerializer.DeserializeAsync<WorkdayRecordAPI>(workdayContent, options);
                */

                return StatusCode(HttpResponseStatus.ACCEPTED);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode(HttpResponseStatus.NOT_FOUND, errorContent);
            }
        }
    }
}
