using AttendanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GeneratingQRCode.Models;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AttendanceTracker.Models.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NuGet.Common;
using AttendanceTracker.Utility;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.SignalR;
using AttendanceTracker.DataAccess.Repository.IRepository;

namespace AttendanceTracker.Controllers
{
    [Area("QR")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IDistributedCache _cache;
        private readonly IHubContext<RefreshHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;
        private static readonly object _sessionTokenLock = new object();

        public HomeController(
            ILogger<HomeController> logger,
            SignInManager<IdentityUser> signInManager,
            IDistributedCache cache,
            IHubContext<RefreshHub> hubContext,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _signInManager = signInManager;
            _cache = cache;
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // Ensures only one thread enters at a time.
            // In case multiple users submitted request at the same time
            lock (_sessionTokenLock)
            {
                // Generate and initialize unique GUID token for the QR code which expires when the user check in or check out
                // Used at the first time the app is loaded
                if (string.IsNullOrEmpty(_cache.GetString(SD.GUID_SESSION)))
                {
                    // Double-check inside the lock
                    if (string.IsNullOrEmpty(_cache.GetString(SD.GUID_SESSION)))
                    {
                        _cache.SetString(SD.GUID_SESSION, GenerateNewToken());
                    }
                }
                GenerateQRCode();
            }
            return View();
        }

        public IActionResult Authentication(string token)
        {
            if (!IsTokenValid(token))
            {
                return UnauthorizedAction("Token has expired. Please rescan the QR code.");
            }

            AuthenticationVM authenticationVM = new()
            {
                Token = token,
            };

            return View(authenticationVM);
        }

        [HttpPost]
        public async Task<IActionResult> Authentication(AuthenticationVM model)
        {
            // Check if token has expired
            if (!IsTokenValid(model.Token))
            {
                return UnauthorizedAction("Token has expired. Please rescan the QR code.");
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Update a new token after each successful authentication for check in/out
                    // Ensures only one thread enters at a time.
                    // In case multiple users submitted request at the same time
                    lock (_sessionTokenLock)
                    {
                        _cache.SetString(SD.GUID_SESSION, GenerateNewToken());
                        GenerateQRCode();
                    }

                    // force refresh the page on all clients
                    await _hubContext.Clients.All.SendAsync("RefreshPage");

                    // Unfortunately, RedirectToAction does not support passing complex object
                    // so we have to make use of TempData[] here and collect the data on the next
                    // RecordAttendance() method
                    TempData["Token"] = model.Token;
                    return RedirectToAction("RecordAttendance", "Home", new { area = "QR" });
                }
                else
                {
                    return RedirectToAction("UnauthorizedAction", "Home",
                        new { area = "QR", message = "Incorrect username and password!" });
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("UnauthorizedAction", "Home",
                new { area = "QR", message = "Something went wrong, please rescan QR and try again..." });
        }

        public IActionResult RecordAttendance()
        {
            AuthenticationVM model = new AuthenticationVM
            {
                Token = TempData["Token"] as string,
                IsCheckIn = UserShouldCheckIn(),
                EmployeeId = _signInManager.UserManager.GetUserId(User)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult RecordAttendance(AuthenticationVM model)
        {
            string errorMessage = string.Empty;
            if (!RecordUserAttendance(model))
            {
                errorMessage = "An error occurred while recording your attendance. Please try again later.";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("UnauthorizedAction", "Home", new { area = "QR" });
            }

            return RedirectToAction("OnSuccessRecord", "Home", new { area = "QR" });
        }

        public IActionResult OnSuccessRecord()
        {
            return View();
        }

        public IActionResult UnauthorizedAction(string message)
        {
            TempData["ErrorMessage"] = message;
            return View("OnFailRecord", message);
        }

        // PRIVATE METHODS BELOW

        private bool IsTokenValid(string token)
        {
            return (!string.IsNullOrEmpty(token)) && (_cache.GetString(SD.GUID_SESSION) == token);
        }

        private string GenerateNewToken()
        {
            return Guid.NewGuid().ToString();
        }

        private void GenerateQRCode()
        {
            // Define and embed token/session into authentication page URL
            string authenticationUrl =
                $"{Url.Action("Authentication", "Home", new { area = "QR" }, Request.Scheme)}?token={_cache.GetString(SD.GUID_SESSION)}";

            // QR Code Generation on Page Load
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeInfo = qrGenerator.CreateQrCode(authenticationUrl, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeInfo);
            byte[] qrCodeBytes = qrCode.GetGraphic(60);

            // Convert QR Code to Base64 URI
            string qrUri = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
            ViewBag.QrCodeUri = qrUri;
        }

        /**
         * @brief Records user attendance by handling check-in and check-out logic.
         * 
         * This function verifies whether the user is attempting a check-in or a check-out,
         * 
         * @param model AuthenticationVM model containing attendance details.
         * @return `true` if success, `false` otherwise.
         */
        private bool RecordUserAttendance(AuthenticationVM model)
        {
            DateTime currDateTime = DateTime.Now;

            if (model.IsCheckIn)
            {
                // Check in
                try
                {
                    _unitOfWork.DailyAttendanceRecord.Add(new DailyAttendanceRecord
                    {
                        Id = currDateTime.ToString("yyyy-MM-dd") + "_" + currDateTime.ToString("HH:mm") + "_" + model.EmployeeId,
                        CheckIn = currDateTime,
                        CheckOut = DateTime.MinValue,
                        EmployeeId = model.EmployeeId
                    });

                    _unitOfWork.Save();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking in for user {UserId} at {DateTime}", model.EmployeeId, currDateTime);
                    return false;
                }
            }
            else
            {
                // Check out
                DailyAttendanceRecord recordToCheckout = FindCheckoutRecord(model.EmployeeId, currDateTime.Date);

                // No pending check-out record found, something went wrong
                if (recordToCheckout == null)
                {
                    _logger.LogWarning("No pending check-out record found for user {UserId} at {DateTime}", model.EmployeeId, currDateTime);
                    throw new InvalidOperationException("No pending check-out record found. But user already check-in.");
                }

                try
                {
                    recordToCheckout.CheckOut = currDateTime;

                    _unitOfWork.DailyAttendanceRecord.Update(recordToCheckout);
                    _unitOfWork.Save();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking out for user {UserId} at {DateTime}", model.EmployeeId, currDateTime);
                    return false;
                }
            }

            // successfully recorded
            return true;
        }

        /**
         * @brief Finds the record should be checked out for a specific employee on current date.
         * 
         * This method retrieves the daily attendance record of an employee that should be updated
         * for check-out process. The record will have user who has checked in 
         * but has not checked out yet on the current date.
         * 
         * @param employeeId The unique identifier of the employee.
         * @param currDateTime The current date-time used to filter records.
         *
         * @return DailyAttendanceRecord The attendance record of the employee if found, otherwise null.
         */
        private DailyAttendanceRecord FindCheckoutRecord(string employeeId, DateTime currDateTime)
        {
            var userAttendanceRecord = _unitOfWork.DailyAttendanceRecord.Get(
                a => (a.EmployeeId == employeeId) && (a.CheckIn.Date == currDateTime.Date) && (a.CheckOut == DateTime.MinValue),
                includeProperties: "Employee");

            return userAttendanceRecord;
        }

        /**
         * @brief Determines if the current user should check in based on attendance records.
         * 
         * @details This method retrieves attendance records for the logged-in user on the current date.
         *          It checks if there are any existing records with an unset check-out time.
         *          If such a record exists, the user is considered already checked in and should not check in again.
         *
         * @note The method assumes that `CheckOut == DateTime.MinValue` indicates an unchecked-out record.
         *
         * @return Returns `true` if the user should check in, otherwise `false`.
         */
        private bool UserShouldCheckIn()
        {
            string currentUserId = _signInManager.UserManager.GetUserId(User);
            var userAttendanceRecords = _unitOfWork.DailyAttendanceRecord.GetAll(
                a => a.EmployeeId == currentUserId && a.CheckIn.Date == DateTime.Today, includeProperties: "Employee");

            bool userShouldCheckIn = true;

            if (userAttendanceRecords == null)
            {
                // No check-in records found for today, so this is user's first check-in of the day
                return userShouldCheckIn;
            }

            // When enter here, it means the user has already checked in today
            // Search if there are any records pending for check out. Otherwise, this is a new check-in request
            foreach (var record in userAttendanceRecords)
            {
                if (record.CheckOut == DateTime.MinValue)
                {
                    // Invalid check out record indicating that there is a pending check out
                    // So abort the check-in process
                    userShouldCheckIn = false;
                    break;
                }
            }

            return userShouldCheckIn;
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetUserDailyRecord(string employeeId)
        {
            var userDailyAttendanceRecords =
                _unitOfWork.DailyAttendanceRecord.GetAll(filter: a => a.EmployeeId == employeeId && a.CheckIn.Date == DateTime.Today);

            var dailyRecords = userDailyAttendanceRecords.Select(a => new DailyAttendanceVM
                {
                    CheckIn = a.CheckIn.ToString("HH:mm tt"),
                    CheckOut = a.CheckOut.ToString("HH:mm tt"),
                    EmployeeId = a.EmployeeId,
                });

            return Json(new { data = dailyRecords });
        }

        #endregion
    }
}
