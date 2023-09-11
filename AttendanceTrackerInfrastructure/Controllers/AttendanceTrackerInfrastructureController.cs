using APIClasses;
using AttendanceTrackerInfrastructure.Migrations;
using AttendanceTrackerInfrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceTrackerInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceTrackerInfrastructureController : ControllerBase
    {
        // private readonly AttendanceTrackerDbContext _dbContext;
        public readonly IConfiguration _configuration;

        // public AttendanceTrackerInfrastructureController(AttendanceTrackerDbContext dbContext)
        public AttendanceTrackerInfrastructureController(IConfiguration configuration)
        {
            _configuration = configuration;
            // _dbContext = dbContext;
        }

        [HttpGet]
        [Route("get-admins")]
        public IActionResult GetAdmins() 
        {
            SqlConnection conn;
            SqlDataAdapter adapter;
            DataTable dt;
            try
            {
                // connecting to db
                conn = new SqlConnection(_configuration
                    .GetConnectionString("AttendanceTracker")
                    .ToString());

                // building sql query
                adapter = new SqlDataAdapter("SELECT * FROM Admins", conn);

                dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();
            }
            catch (Exception) 
            {
                return StatusCode(500, "Failed to connect to database");
            }


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

        /*

        [HttpGet]
        [Route("get-staffs")]
        public IActionResult GetStaffs()
        {
            List<StaffAPI> staffs = new List<StaffAPI>();
            try
            {
                _dbContext.Database.OpenConnection();
                List<Staff> staffsDb = (from staff in _dbContext.staffs
                                        select staff).ToList();

                // we are returning List<StaffAPI> here
                // that is why we need a static cast the List<Staff> staffsDb from the database
                staffs = staffsDb.Cast<StaffAPI>().ToList();

                return Ok(staffs);
            }
            catch (Exception) 
            {
                return StatusCode(500, "Unable to connect to the database");
            }
        }
        */

        [HttpPost]
        [Route("add-admin")]
        public IActionResult AddAdmin([FromBody] AdminAPI admin)
        {
            SqlConnection conn = new SqlConnection(_configuration
                .GetConnectionString("AttendanceTracker")
                .ToString());

            try
            {
                conn.Open();

                // building sql query
                string sqlScript = "INSERT INTO Admins (Name, Password) VALUES (@Name, @Password);";
                SqlCommand sqlCommand = new SqlCommand(sqlScript, conn);
                sqlCommand.Parameters.AddWithValue("@Name", admin.Name);
                sqlCommand.Parameters.AddWithValue("@Password", admin.Password);

                int rowsAffected = sqlCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return StatusCode(200, "Admin successfully registered");
                }
                else
                {
                    return StatusCode(400, "Internal server error");
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                return StatusCode(500, "Failed to connect to database");
            }
            finally
            {
                conn.Close();
            }
        }

        [HttpPost]
        [Route("add-staff")]
        public IActionResult AddStaff([FromBody] StaffAPI staff)
        {
            SqlConnection conn = new SqlConnection(_configuration
                .GetConnectionString("AttendanceTracker")
                .ToString());

            try
            {
                conn.Open();

                // building sql query
                string sqlScript = "INSERT INTO Staffs(Name, Password, Department) VALUES " +
                    "(@Name, @Password, @Department);";

                SqlCommand sqlCommand = new SqlCommand(sqlScript, conn);
                sqlCommand.Parameters.AddWithValue("@Name", staff.Name);
                sqlCommand.Parameters.AddWithValue("@Password", staff.Password);
                sqlCommand.Parameters.AddWithValue("@Department", staff.Department);

                int rowsAffected = sqlCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return StatusCode(200, "Staff has been registered");
                }
                else
                {
                    return StatusCode(400, "Internal server error");
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                return StatusCode(500, "Failed to connect to database");
            }
            finally
            {
                conn.Close();
            }
        }

        [HttpPost]
        [Route("add-workday-record")]
        public IActionResult AddWorkdayRecord([FromBody] WorkdayRecordAPI workdayRecordAPI)
        {
            SqlConnection conn = new SqlConnection(_configuration
                .GetConnectionString("AttendanceTracker")
                .ToString());

            try
            {
                conn.Open();

                // building sql query
                string sqlScript = "INSERT INTO WorkdayRecords(StaffName, Date, CheckIn, CheckOut) " +
                    " VALUES (@StaffName, @Date, @CheckIn, @CheckOut);";

                SqlCommand sqlCommand = new SqlCommand(sqlScript, conn);
                sqlCommand.Parameters.AddWithValue("@StaffName", workdayRecordAPI.StaffName);
                sqlCommand.Parameters.AddWithValue("@Date", workdayRecordAPI.Date);
                sqlCommand.Parameters.AddWithValue("@CheckIn", workdayRecordAPI.CheckIn);
                sqlCommand.Parameters.AddWithValue("@CheckOut", workdayRecordAPI.CheckOut);

                int rowsAffected = sqlCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    string message = "Workday record for " + workdayRecordAPI.StaffName + " has been updated.";
                    return StatusCode(200, message);
                }
                else
                {
                    return StatusCode(400, "Internal server error");
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                return StatusCode(500, "Failed to connect to database");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
