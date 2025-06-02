using AttendanceTracker.DataAccess.Repository.IRepository;
using AttendanceTracker.Models;
using AttendanceTracker.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AttendanceTracker.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegisterNewEmployee()
        {
            return View();
        }

        public IActionResult WorkingRecord()
        {
            // FIXME: This is a performance overhead.
            // Use a background service to update the database automatically at the end of the day instead.
            // FIXME: End of the fixme comments.
            UpdateDatabase();
            return View();
        }

        public IActionResult ViewAttendanceRecordsOfEmployee(string employeeId)
        {
            ViewBag.EmployeeId = employeeId;
            return View();
        }

        // PRIVATE METHODS
        private void UpdateDatabase()
        {
            var allEmployees = _unitOfWork.ApplicationEmployees.GetAll();

            foreach (var employee in allEmployees)
            {
                DateTime lastWorkingDate = FindEmployeeLastWorkingDate(employee.Id);

                if (lastWorkingDate != DateTime.MinValue && ShouldUpdateAttendance(employee.Id, lastWorkingDate))
                {
                    // There is multiple check-in and check-out records for the same employee on the same date.
                    // Used the check-out date to find the last working date of the employee instead of the check-in
                    // to rule out the possibility of an employee not checking out after checking in.
                    IEnumerable<DailyAttendanceRecord>? lastWorkingRecordsOfCurrentEmployee =
                        _unitOfWork.DailyAttendanceRecord.GetAll(
                            a => (a.EmployeeId == employee.Id) && (a.CheckOut.Date == lastWorkingDate.Date));

                    // After we retrieve all check-in and check-out records for the same date
                    // Calculate the total working hours for each check-in and check-out record
                    double totalWorkingHours = 0;
                    foreach (var record in lastWorkingRecordsOfCurrentEmployee)
                    {
                        TimeSpan duration = record.CheckOut - record.CheckIn;
                        totalWorkingHours += duration.TotalHours;
                    }

                    // Now we got the total working hours for current employee on the day
                    // time to update it in the Attendance table
                    _unitOfWork.Attendance.Add(new Attendance
                    {
                        Id = Guid.NewGuid().ToString(),
                        TotalWorkingHours = totalWorkingHours,
                        RecordDate = lastWorkingDate,
                        EmployeeId = employee.Id
                    });

                    _unitOfWork.Save();
                }
            }
        }

        /**
         * @brief Determines whether attendance should be updated for a given employee.
         *
         * This method checks if an attendance record already exists for the given employee
         * on the last working date. If an existing record is found, prevents it from redundant updates.
         *
         * @param employeeId The unique identifier of the employee.
         * @param lastWorkingDate The last recorded working date to check against attendance records.
         *
         * @return true if attendance should be updated; false if an existing record is found for the date.
         */
        private bool ShouldUpdateAttendance(string employeeId, DateTime lastWorkingDate)
        {
            var attendances = _unitOfWork.Attendance.GetAll(a => a.EmployeeId == employeeId);

            // If we found attendance has been recorded for this employee,
            // check if the last working date is already recorded, if so, avoid updating again.
            foreach (var attendance in attendances)
            {
                if (attendance.RecordDate.Date == lastWorkingDate.Date)
                {
                    return false; // Attendance record already exists for this date
                }
            }

            return true;
        }

        /**
         * @brief Determines the last working date of an employee based on check-in and check-out records.
         * 
         * This method retrieves all daily attendance records for a given employee, 
         * sorts them in descending order by check-in time, and returns the most recent check-in date.
         *
         * Following conditions will return `DateTime.MinValue`:
         *  - No valid check-in found
         *  - Valid check-in found but no check-out recorded (user in pending check-out state)
         *
         * @param employeeId The unique identifier of the employee.
         * @return The last recorded working date of the employee,
         *         or `DateTime.MinValue` if no records exist or pending check-out exist.
         *
         * @note The method filters records by EmployeeId and includes Employee details.
         * @warning If the employee has a pending check-out or has never checked in, the function will return `DateTime.MinValue`.
         *          This prevents updating the Attendance table prematurely.
         */
        private DateTime FindEmployeeLastWorkingDate(string employeeId)
        {
            var currentEmployeeDailyRecords = _unitOfWork.DailyAttendanceRecord.GetAll(
                filter: a => a.EmployeeId == employeeId,
                includeProperties: "Employee");

            if (currentEmployeeDailyRecords != null && currentEmployeeDailyRecords.Any())
            {
                DailyAttendanceRecord record =
                    currentEmployeeDailyRecords.OrderByDescending(a => a.CheckIn).FirstOrDefault();

                if (record == null || record.CheckOut == DateTime.MinValue)
                {
                    // If the employee has no check-in or check-out records, return invalid date
                    // to avoid updating the Attendance table.
                    // Also, check if there is any pending check-out record. If there is, abort the update as well.
                    return DateTime.MinValue;
                }

                return record.CheckIn.Date;
            }

            // This employee is new, return invalid date so we do not update it
            // in the Attendance table. Employee will need to check in first.
            return DateTime.MinValue;
        }

        // This allow API to be called from external applications
        #region API CALLS

        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            List<ApplicationEmployee> employees = _unitOfWork.ApplicationEmployees.GetAll().ToList();

            // Filter out Admin users
            List<ApplicationEmployee> nonAdminEmployees =
                employees.Where(a => !_userManager.IsInRoleAsync(a, "Admin").GetAwaiter().GetResult()).ToList();

            var employeesData = nonAdminEmployees.Select(a => new AttendanceVM
                {
                    EmployeeId = a.Id,
                    Email = a.UserName
                });

            return Json(new { data = employeesData });
        }
        
        [HttpGet]
        public IActionResult GetAllAttendanceRecordsOfEmployee(string employeeId)
        {
            List<Attendance> currEmployeeRecords = _unitOfWork.Attendance.GetAll(
                a => a.EmployeeId == employeeId, includeProperties: "Employee").ToList();

            List<AttendanceVM> attendancesData =
                currEmployeeRecords.Select(a => new AttendanceVM
                {
                    Email = a.Employee.UserName,
                    Date = a.RecordDate.ToString("yyyy-MM-dd"),
                    TotalWorkingHours = a.TotalWorkingHours
                }).ToList();

            return Json(new {data = attendancesData});
        }

        #endregion
    }
}