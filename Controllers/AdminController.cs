using AttendanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AttendanceTracker.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult RegisterNewEmployee()
        {
            return View();
        }
    }
}