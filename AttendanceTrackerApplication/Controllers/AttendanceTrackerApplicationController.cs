using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
        public async IActionResult GetAdmins()
        {
            string route = _apiurl + "/get-admins";
            var response = await _httpClient.GetAsync(route);

            if ((int)(response.StatusCode) == )
            {

            }
        }
    }
}
