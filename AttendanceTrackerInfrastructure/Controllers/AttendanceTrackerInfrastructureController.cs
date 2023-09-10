using APIClasses;
using AttendanceTrackerInfrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceTrackerInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceTrackerInfrastructureController : ControllerBase
    {
        public readonly IConfiguration _configuration;

        public AttendanceTrackerInfrastructureController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("get-admins")]
        public IActionResult GetAdmins() 
        {
            SqlConnection conn;
            SqlDataAdapter adapter;
            try
            {
                // connecting to db
                conn = new SqlConnection(_configuration
                    .GetConnectionString("AttendanceTracker")
                    .ToString());

                // building sql query
                adapter = new SqlDataAdapter("SELECT * FROM Admins", conn);
            }
            catch (Exception) 
            {
                return StatusCode(500, "Failed to connect to database");
            }

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            List<AdminAPI> admins = new List<AdminAPI>();

            // check if db has any data
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    AdminAPI admin = new AdminAPI();
                    admin.Name = dt.Rows[i]["Name"].ToString();
                    admin.Password = dt.Rows[i]["Password"].ToString();
                    admins.Add(admin);
                }
            }

            if (admins.Count > 0)
            {
                return Ok(admins);
            }
            else
            {
                return StatusCode(404, "No data has been found");
            }
        }
    }
}
