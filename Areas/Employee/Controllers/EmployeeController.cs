using AttendanceTracker.DataAccess.Repository.IRepository;
using AttendanceTracker.Models;
using AttendanceTracker.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.Diagnostics;

namespace AttendanceTracker.Controllers
{
    [Area("Employee")]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeController(ILogger<EmployeeController> logger,
                                  SignInManager<IdentityUser> signInManager,
                                  IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            string employeeId = _signInManager.UserManager.GetUserId(User);

            ApplicationEmployee employee = _unitOfWork.ApplicationEmployees.Get(filter: a => a.Id == employeeId);
            string employeeName = employee.Email;

            ViewBag.EmployeeId = employeeId;
            ViewBag.EmployeeName = employeeName;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // PRIVATE METHODS

        #region API CALLS

        [HttpGet]
        public IActionResult GetEmployeeAttendances(string employeeId)
        {
            var records =
                _unitOfWork.Attendance.GetAll(filter: a => a.EmployeeId == employeeId, includeProperties: "Employee");

            List<AttendanceVM> attendances = new List<AttendanceVM>();

            foreach (var record in records)
            {
                if (DateTime.Now.Year == record.RecordDate.Year &&
                    DateTime.Now.Month == record.RecordDate.Month)
                {
                    AttendanceVM attendance = new()
                    {
                        Date = record.RecordDate.ToString("yyyy-MM-dd"),
                        TotalWorkingHours = record.TotalWorkingHours
                    };

                    attendances.Add(attendance);
                }
            }

            return Json(new { data = attendances });
        }

        #endregion
    }
}
