using AttendanceTracker.DataAccess.Repository.IRepository;
using AttendanceTracker.Models;
using AttendanceTracker.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Diagnostics;
using System.Globalization;

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
            return RedirectToAction("DisplayQRCode", "Home", new { area = "QR" });
        }

        public IActionResult RegisterNewEmployee()
        {
            return View();
        }

        /**
        * @brief View all employees working hours with the latest data
        */
        public IActionResult WorkingRecord()
        {
            // FIXME: This is a performance overhead.
            // Use a background service to update the database automatically at the end of the day instead.
            // FIXME: End of the fixme comments.
            UpdateDatabase();
            return View();
        }

        /**
        * @brief View the given employee Attendance records
        */
        public IActionResult ViewAttendanceRecordsOfEmployee(string employeeId)
        {
            ViewBag.EmployeeId = employeeId;
            return View();
        }

        /**
        * @brief Display a list of records to be selected to edit
        */
        public IActionResult EditAttendanceRecord(string employeeId, string date)
        {
            DateTime dateTime = DateTime.Parse(date);
            DailyAttendanceRecord record =
                _unitOfWork.DailyAttendanceRecord.Get(
                    filter: a => a.EmployeeId == employeeId && a.CheckIn.Date == dateTime.Date,
                    includeProperties: "Employee");

            DailyAttendanceVM dailyAttendanceVM = new()
            {
                Id = record.Id,
                Date = dateTime,
                EmployeeId = record.EmployeeId,
                CheckIn = record.CheckIn.ToString("HH:mm tt"),
                CheckOut = record.CheckOut.ToString("HH:mm tt"),
                EmployeeName = record.Employee.UserName
            };
            
            return View(dailyAttendanceVM);
        }

        [HttpPost]
        public IActionResult EditAttendanceRecord(DailyAttendanceVM model)
        {
            var record = _unitOfWork.DailyAttendanceRecord.Get(filter: a => a.Id == model.Id && a.CheckIn.Date == model.Date);

            if (record == null)
            {
                throw new NullReferenceException("Record cannot be found! Something went wrong");
            }

            // Parse the check-in time from DailyAttendanceVM which is in string format
            // and construct a DB compatible DateTime object to update the DB
            DateTime parsedCheckIn = DateTime.ParseExact(model.CheckIn, "HH:mm", CultureInfo.InvariantCulture);
            DateTime updateCheckIn = new DateTime(
                record.CheckIn.Year,
                record.CheckIn.Month,
                record.CheckIn.Day,
                parsedCheckIn.Hour,
                parsedCheckIn.Minute,
                0
            );

            // Parse the check-out time from DailyAttendanceVM which is in string format
            // and construct a DB compatible DateTime object to update the DB
            DateTime parsedCheckOut = DateTime.ParseExact(model.CheckOut, "HH:mm", CultureInfo.InvariantCulture);
            DateTime updateCheckOut = new DateTime(
                record.CheckOut.Year,
                record.CheckOut.Month,
                record.CheckOut.Day,
                parsedCheckOut.Hour,
                parsedCheckOut.Minute,
                0
            );

            TimeSpan duration = updateCheckOut - updateCheckIn;
            double totalWorkingHours = duration.TotalHours;

            if (totalWorkingHours < 0)
            {
                TempData["error"] = "Check in cannot be later than check out!";
                return RedirectToAction("EditAttendanceRecord", "Admin",
                    new { area = "Admin", employeeId = model.EmployeeId, date = model.Date.ToString("yyyy-MM-dd") });
            }

            // After setting up the updateCheckIn and updateCheckout, update the record
            record.CheckIn = updateCheckIn;
            record.CheckOut = updateCheckOut;

            _unitOfWork.DailyAttendanceRecord.Update(record);
            _unitOfWork.Save();

            UpdateAttendanceDatabase(model);

            TempData["success"] = "Updated record successfully!";
            return RedirectToAction("DisplayQRCode", "Home", new { area = "QR" });
        }

        public IActionResult DeleteEmployee(string employeeId)
        {
            ApplicationEmployee employee = _unitOfWork.ApplicationEmployees.Get(filter: a => a.Id == employeeId);
            EmployeeVM employeeVM = new()
            {
                Id = employee.Id,
                EmployeeName = employee.UserName,
                Email = employee.Email
            };

            return View(employeeVM);
        }

        [HttpPost]
        public IActionResult DeleteEmployee(EmployeeVM employee)
        {
            string employeeIdToDelete = employee.Id;
            ApplicationEmployee employeeToDelete =
                _unitOfWork.ApplicationEmployees.Get(filter: a => a.Id == employeeIdToDelete);

            _unitOfWork.ApplicationEmployees.Remove(employeeToDelete);
            _unitOfWork.Save();

            TempData["success"] = "Employee successfully removed!";
            return RedirectToAction("DisplayQRCode", "Home", new { area = "QR" });
        }

        // PRIVATE METHODS
        private void UpdateDatabase()
        {
            var allEmployees = _unitOfWork.ApplicationEmployees.GetAll();

            foreach (var employee in allEmployees)
            {
                DateTime lastWorkingDate = FindEmployeeLastWorkingDate(employee.Id);

                // lastWorkingDate will be DateTime.MinValue if there is a pending check out
                // and if the record already exist, abort the update as well.
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

        private void UpdateAttendanceDatabase(DailyAttendanceVM model)
        {
            Attendance record = 
                _unitOfWork.Attendance.Get(filter: a => a.EmployeeId == model.EmployeeId && a.RecordDate == model.Date);

            DateTime checkIn = DateTime.Parse(model.CheckIn);
            DateTime checkOut = DateTime.Parse(model.CheckOut);
            TimeSpan duration = checkOut - checkIn;

            record.TotalWorkingHours = duration.TotalHours;

            _unitOfWork.Attendance.Update(record);
            _unitOfWork.Save();
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

            // This employee is new to the company, since there are no working attendance records at all.
            // return invalid date so we do not update it in the Attendance table.
            // Employee will need to check in first.
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