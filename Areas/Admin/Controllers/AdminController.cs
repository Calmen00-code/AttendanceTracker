using AttendanceTracker.DataAccess.Repository.IRepository;
using AttendanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AttendanceTracker.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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


        // PRIVATE METHODS
        private void UpdateDatabase()
        {
            var allEmployees = _unitOfWork.ApplicationEmployees.GetAll();

            foreach (var employee in allEmployees)
            {
                DateTime lastWorkingDate = FindEmployeeLastWorkingDate(employee.Id);

                // There is multiple check-in and check-out records for the same employee on the same date.
                // Used the check-out date to find the last working date of the employee instead of the check-in
                // to rule out the possibility of an employee not checking out after checking in.
                IEnumerable<DailyAttendanceRecord>? lastWorkingRecordsOfCurrentEmployee =
                    _unitOfWork.DailyAttendanceRecord.GetAll(
                        a => (a.EmployeeId == employee.Id) && (a.CheckOut.Date == lastWorkingDate));


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
    }
}