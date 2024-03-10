using APIClasses;
using AttendanceTrackerInfrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceTrackerInfrastructure.Controllers
{
    // for development
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

            // connecting to db
            conn = new SqlConnection(_configuration.GetConnectionString("AttendanceTracker").ToString());

            try
            {

                // building sql query
                adapter = new SqlDataAdapter("SELECT * FROM Admins", conn);

                dt = new DataTable();
                adapter.Fill(dt);
            }
            catch (Exception) 
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
            }
            finally
            {
                conn.Close();
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
                return StatusCode(HttpResponseStatus.NOT_FOUND, "No data has been found");
            }
        }

        [HttpGet]
        [Route("get-staff/{name}")]
        public IActionResult GetStaff(string name) 
        {
            SqlConnection conn;
            SqlDataAdapter adapter;
            DataTable dt;

            // connecting to db
            conn = new SqlConnection(_configuration.GetConnectionString("AttendanceTracker").ToString());

            try
            {
                // building sql query
                string sqlQuery = "SELECT * FROM Staffs WHERE Name = \'" + name + "\';";
                adapter = new SqlDataAdapter(sqlQuery, conn);

                dt = new DataTable();
                adapter.Fill(dt);
            }
            catch (Exception) 
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
            }
            finally
            {
                conn.Close();
            }

            // check if db has any data
            if (dt.Rows.Count == 0)
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "No data has been found");
            }

            StaffAPI staff = new StaffAPI();
            staff.Name = dt.Rows[0]["Name"].ToString();
            staff.Password = dt.Rows[0]["Password"].ToString();
            staff.Department = dt.Rows[0]["Department"].ToString();

            return Ok(staff);
        }

        [HttpGet]
        [Route("get-admin/{adminName}")]
        public IActionResult GetAdmin(string adminName) 
        {
            SqlConnection conn;
            SqlDataAdapter adapter;
            DataTable dt;

            // connecting to db
            conn = new SqlConnection(_configuration.GetConnectionString("AttendanceTracker").ToString());

            try
            {
                // building sql query
                string sqlQuery = "SELECT * FROM Admins WHERE Name = \'" + adminName + "\';";
                adapter = new SqlDataAdapter(sqlQuery, conn);

                dt = new DataTable();
                adapter.Fill(dt);
                // conn.Close();
            }
            catch (Exception) 
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
            }
            finally
            { 
                conn.Close(); 
            }

            // check if db has any data
            if (dt.Rows.Count == 0)
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "No data has been found");
            }

            AdminAPI admin = new AdminAPI();
            admin.Name = dt.Rows[0]["Name"].ToString();
            admin.Password = dt.Rows[0]["Password"].ToString();

            return Ok(admin);
        }

        [HttpGet]
        [Route("get-staffs")]
        public IActionResult GetStaffs() 
        {
            SqlConnection conn;
            SqlDataAdapter adapter;
            DataTable dt;

            // connecting to db
            conn = new SqlConnection(_configuration.GetConnectionString("AttendanceTracker").ToString());

            try
            {
                // building sql query
                adapter = new SqlDataAdapter("SELECT * FROM Staffs", conn);

                dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();
            }
            catch (Exception) 
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
            }
            finally
            { 
                conn.Close(); 
            }

            List<StaffAPI> staffs = new List<StaffAPI>();

            // check if db has any data
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StaffAPI staff = new StaffAPI();
                    staff.Name = dt.Rows[i]["Name"].ToString();
                    staff.Password = dt.Rows[i]["Password"].ToString();
                    staff.Department = dt.Rows[i]["Department"].ToString();
                    staffs.Add(staff);
                }
            }

            if (staffs.Count > 0)
            {
                return Ok(staffs);
            }
            else
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "No data has been found");
            }
        }

        [HttpGet]
        [Route("is-staff-exist/{name}")]
        public IActionResult IsStaffExist(string name)
        {

            SqlConnection conn;
            SqlDataAdapter adapter;
            DataTable dt;

            // connecting to db
            conn = new SqlConnection(_configuration.GetConnectionString("AttendanceTracker").ToString());

            try
            {
                // building sql query
                adapter = new SqlDataAdapter("SELECT * FROM Staffs", conn);

                dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();
            }
            catch (Exception) 
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
            }
            finally
            {
                conn.Close();
            }

            List<string> staffs = new List<string>();

            // check if db has any data
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string staff = "";
                    staff = dt.Rows[i]["Name"].ToString();
                    staffs.Add(staff);
                }
            }

            if (staffs.Contains(name))
            {
                return Ok();
            }
            else
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "Name does not exist");
            }
        }

        [HttpGet]
        [Route("get-departments")]
        public IActionResult GetDepartments() 
        {
            SqlConnection conn;
            SqlDataAdapter adapter;
            DataTable dt;

            // connecting to db
            conn = new SqlConnection(_configuration.GetConnectionString("AttendanceTracker").ToString());

            try
            {
                // connecting to db
                conn = new SqlConnection(_configuration
                    .GetConnectionString("AttendanceTracker")
                    .ToString());

                // building sql query
                adapter = new SqlDataAdapter("SELECT * FROM Departments", conn);

                dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();
            }
            catch (Exception) 
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
            }
            finally
            {
                conn.Close ();
            }


            List<DepartmentAPI> departments = new List<DepartmentAPI>();

            // check if db has any data
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DepartmentAPI department = new DepartmentAPI();
                    department.Name = dt.Rows[i]["Name"].ToString();
                    departments.Add(department);
                }
            }

            if (departments.Count > 0)
            {
                return Ok(departments);
            }
            else
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "No data has been found");
            }
        }

        [HttpGet]
        [Route("get-department/{departmentName}")]
        public IActionResult GetDepartment(string departmentName) 
        {
            SqlConnection conn;
            SqlDataAdapter adapter;
            DataTable dt;

            // connecting to db
            conn = new SqlConnection(_configuration.GetConnectionString("AttendanceTracker").ToString());

            try
            {
                // connecting to db
                conn = new SqlConnection(_configuration
                    .GetConnectionString("AttendanceTracker")
                    .ToString());

                // building sql query
                string sqlQuery = "SELECT * FROM Departments WHERE Name = \'" + departmentName + "\';";
                adapter = new SqlDataAdapter(sqlQuery, conn);

                dt = new DataTable();
                adapter.Fill(dt);
                // conn.Close();
            }
            catch (Exception e) 
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database, " + e.Message);
            }
            finally 
            { 
                conn.Close(); 
            }

            // check if db has any data
            if (dt.Rows.Count == 0)
            {
                return StatusCode(HttpResponseStatus.NOT_FOUND, "No data has been found");
            }

            DepartmentAPI department = new DepartmentAPI();
            department.Name = dt.Rows[0]["Name"].ToString();

            return Ok(department);
        }

        [HttpGet]
        [Route("get-workdays")]
        public IActionResult GetWorkdays() 
        { 
            SqlConnection conn;
            SqlDataAdapter adapter;
            DataTable dt;

            // connecting to db
            conn = new SqlConnection(_configuration.GetConnectionString("AttendanceTracker").ToString());

            try
            {
                // connecting to db
                conn = new SqlConnection(_configuration
                    .GetConnectionString("AttendanceTracker")
                    .ToString());

                // building sql query
                adapter = new SqlDataAdapter("SELECT * FROM WorkdayRecords", conn);

                dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();
            }
            catch (Exception) 
            {
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
            }
            finally
            { 
                conn.Close(); 
            }

            List<WorkdayRecordAPI> workdayRecords = new List<WorkdayRecordAPI>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    WorkdayRecordAPI workdayRecord = new WorkdayRecordAPI();
                    workdayRecord.StaffName = dt.Rows[i]["StaffName"].ToString();
                    workdayRecord.TotalWorkingHours = (decimal)(dt.Rows[i]["TotalWorkingHours"]);

                    // The format of the date coming from SQL database is not aligned with DateTime type in ASP.NET. Need to do parsing here
                    // Otherwise we will get exception error
                    workdayRecord.Date = DateTime.ParseExact((dt.Rows[i]["Date"]).ToString(), "d/M/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    workdayRecord.CheckIn = DateTime.ParseExact(dt.Rows[i]["CheckIn"].ToString(), "d/M/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    workdayRecord.CheckOut = DateTime.ParseExact(dt.Rows[i]["CheckOut"].ToString(), "d/M/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    workdayRecords.Add(workdayRecord);
                }
            }

            return Ok(workdayRecords);
        }

        /***
         * TODO TEMPORARY: using non-LINQ method to query (pure string)
         */
        /*
        [HttpGet]
        [Route("get-staffs")]
        public IActionResult GetStaffs()
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
                adapter = new SqlDataAdapter("SELECT * FROM Staffs", conn);

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

        /***
         * TODO : using LINQ method to query INSTEAD OF (pure string)
         */
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
                    return StatusCode(HttpResponseStatus.CREATED, "Admin successfully registered");
                }
                else
                {
                    return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, 
                        "Internal server error, no rows affected");
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
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
                System.Diagnostics.Debug.WriteLine("Department in DB: " + staff.Department);
                sqlCommand.Parameters.AddWithValue("@Department", staff.Department);

                int rowsAffected = sqlCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return StatusCode(HttpResponseStatus.CREATED, "Staff has been registered");
                }
                else
                {
                    return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, 
                        "Internal server error, no rows affected");
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, 
                    "Failed to connect to database");
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
                string sqlScript = "INSERT INTO WorkdayRecords(StaffName, Date, CheckIn, CheckOut, TotalWorkingHours) " +
                    " VALUES (@StaffName, @Date, @CheckIn, @CheckOut, @TotalWorkingHours);";

                SqlCommand sqlCommand = new SqlCommand(sqlScript, conn);
                sqlCommand.Parameters.AddWithValue("@StaffName", workdayRecordAPI.StaffName);
                sqlCommand.Parameters.AddWithValue("@Date", workdayRecordAPI.Date);
                sqlCommand.Parameters.AddWithValue("@CheckIn", workdayRecordAPI.CheckIn);
                sqlCommand.Parameters.AddWithValue("@CheckOut", workdayRecordAPI.CheckOut);
                sqlCommand.Parameters.AddWithValue("@TotalWorkingHours", workdayRecordAPI.TotalWorkingHours);

                int rowsAffected = sqlCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    string message = "Workday record for " + workdayRecordAPI.StaffName + " has been updated.";
                    return StatusCode(HttpResponseStatus.CREATED, message);
                }
                else
                {
                    return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, 
                        "Internal server error, no rows affected");
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
            }
            finally
            {
                conn.Close();
            }
        }

        [HttpPost]
        [Route("add-department")]
        public IActionResult AddDepartment([FromBody] DepartmentAPI departmentAPI)
        {
            SqlConnection conn = new SqlConnection(_configuration
                .GetConnectionString("AttendanceTracker")
                .ToString());

            try
            {
                conn.Open();

                string sqlScript = "INSERT INTO Departments(Name) " + " VALUES(@Name);";

                SqlCommand sqlCommand = new SqlCommand(sqlScript, conn);
                sqlCommand.Parameters.AddWithValue("@Name", departmentAPI.Name);

                int rowsAffected = sqlCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    string message = "Department " + departmentAPI.Name + " has been added";
                    return StatusCode(HttpResponseStatus.CREATED, message);
                }
                else
                {
                    return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR,
                        "Internal server error, no rows affected");
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, "Failed to connect to database");
            }
            finally
            {
                conn.Close();
            }
        }

        [HttpPut]
        [Route("update-workday")]
        public IActionResult UpdateWorkday([FromBody] WorkdayRecordAPI workdayRecordAPI)
        {
            SqlConnection conn = new SqlConnection(_configuration
                .GetConnectionString("AttendanceTracker")
                .ToString());

            try
            {
                conn.Open();

                string dateStr = workdayRecordAPI.Date.ToString("yyyy-MM-dd");
                string sqlScript = "UPDATE WorkdayRecords" +
                    " SET CheckIn = \'" + workdayRecordAPI.CheckIn + "\', CheckOut = \'" + workdayRecordAPI.CheckOut + "\'," +
                    " TotalWorkingHours = " + workdayRecordAPI.TotalWorkingHours +
                    " WHERE StaffName = \'" + workdayRecordAPI.StaffName + "\' AND CAST(Date AS DATE) = \'" + dateStr + "\';"; 

                SqlCommand sqlCommand = new SqlCommand(sqlScript, conn);

                int rowsAffected = sqlCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    string message = "Workday record for " + workdayRecordAPI.StaffName + " has been updated.";
                    return StatusCode(HttpResponseStatus.ACCEPTED, message);
                }
                else
                {
                    return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR,
                        "Internal server error, no rows affected");
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                return StatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR, e.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
