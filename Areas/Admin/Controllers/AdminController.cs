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

                if (ShouldUpdateAttendance(employee.Id, lastWorkingDate))
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
                        Date = lastWorkingDate,
                        EmployeeId = employee.Id
                    });

                    _unitOfWork.Save();
                }
            }
        }


        private bool ShouldUpdateAttendance(string employeeId, DateTime lastWorkingDate)
        {
            var attendances = _unitOfWork.Attendance.GetAll(a => a.EmployeeId == employeeId);
            foreach (var attendance in attendances)
            {
                if (attendance.Date.Date == lastWorkingDate.Date)
                {
                    return false; // Attendance record already exists for this date
                }
            }
            return true;
        }

        private DateTime FindEmployeeLastWorkingDate(string employeeId)
        {
            var currentEmployeeDailyRecords = _unitOfWork.DailyAttendanceRecord.GetAll(
                filter: a => a.EmployeeId == employeeId,
                includeProperties: "Employee");

            if (currentEmployeeDailyRecords.Any())
            {
                DailyAttendanceRecord record =
                    currentEmployeeDailyRecords.OrderByDescending(a => a.CheckIn).FirstOrDefault();

                if (record == null)
                {
                    return DateTime.MinValue;
                }

                return record.CheckIn.Date;
            }

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
                    Date = a.Date.ToString("yyyy-MM-dd"),
                    TotalWorkingHours = a.TotalWorkingHours
                }).ToList();

            return Json(new {data = attendancesData});
        }

        #endregion
    }
}