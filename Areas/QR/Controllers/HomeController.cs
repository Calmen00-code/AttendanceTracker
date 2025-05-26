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
using AttendanceTracker.AttendanceTrackerStateMachine;

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
        private readonly AttendanceTrackerStateContext _attendanceTracker;

        private static readonly object _sessionTokenLock = new object();

        public HomeController(
            ILogger<HomeController> logger,
            SignInManager<IdentityUser> signInManager,
            IDistributedCache cache,
            IHubContext<RefreshHub> hubContext,
            IUnitOfWork unitOfWork,
            AttendanceTrackerStateContext attendanceTracker)
        {
            _logger = logger;
            _signInManager = signInManager;
            _cache = cache;
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
            _attendanceTracker = attendanceTracker;
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
                AttendanceTrackerState = _attendanceTracker.GetCurrentState().GetStateIdentifier()
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
                    // FIXME: remove this when state machine is implemented
                    // string errorMessage = RecordUserAttendance(model);

                    // if (!string.IsNullOrEmpty(errorMessage))
                    // {
                    //     return UnauthorizedAction(errorMessage);
                    // }
                    // FIXME:

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
            return View();
        }

        public IActionResult OnSuccessRecord()
        {
            return View();
        }

        public IActionResult UnauthorizedAction(string message)
        {
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
         * ensures proper attendance tracking, and prevents duplicate check-ins.
         * 
         * @param model AuthenticationVM model containing attendance details.
         * @return A string message is EMPTY when success. Otherwise, error message will be returned
         */
        private string RecordUserAttendance(AuthenticationVM model)
        {
            // Check if user tries to check out without checking in for the day
            if (!model.IsCheckIn && !UserAlreadyCheckedIn(DateTime.Today))
            {
                return "You have not checked in for the day yet. Please rescan QR and check in first.";
            }

            Attendance userAttendance = 
                _unitOfWork.Attendance.Get(a => a.EmployeeId == _signInManager.UserManager.GetUserId(User));

            if (model.IsCheckIn)
            {
                // Check in
                if (UserAlreadyCheckedIn(DateTime.Today))
                {
                    // User has already checked in for the day,
                    // so update the record instead of adding a new one
                    // First check if user tries to check in again without checking out

                    // FIXME: We have updated a new DB schema, this has to be updated as it is no longer valid
                    /*
                    if (userAttendance.CheckOut == DateTime.MinValue)
                    {
                        return "You have already checked in for work today at " + userAttendance.CheckIn.ToString("HH:mm:ss");
                    }
                    */
                }
                else
                {
                    // First time check in for the day
                    string userId = _signInManager.UserManager.GetUserId(User);
                    string attendanceId = (DateTime.Now.ToString("yyyyMMdd")) + "_" + userId;

                    // User has not checked in for the day yet, so add a new record
                    // FIXME: We have updated a new DB schema, this has to be updated as it is no longer valid
                    /*
                    _unitOfWork.Attendance.Add(new Attendance
                    {
                        Id = attendanceId,
                        CheckIn = DateTime.Now,
                        CheckOut = DateTime.MinValue,
                        TotalWorkingHours = 0,
                        EmployeeId = _signInManager.UserManager.GetUserId(User)
                    });

                    _unitOfWork.Save();
                    */
                }
            }
            else
            {
                // Check out
                // FIXME: We have updated a new DB schema, this has to be updated as it is no longer valid
                /*
                userAttendance.CheckOut = DateTime.Now;
                userAttendance.TotalWorkingHours = 
                    (userAttendance.CheckOut - userAttendance.CheckIn).TotalHours;
                _unitOfWork.Attendance.Update(userAttendance);
                */
            }

            // successfully recorded
            return String.Empty;
        }

        private bool UserAlreadyCheckedIn(DateTime date)
        {
            // Check if the user has already checked in for the day
            // FIXME: We have updated a new DB schema, this has to be updated as it is no longer valid
            /*
            var attendanceToday = _unitOfWork.Attendance.Get(
                a => a.EmployeeId == _signInManager.UserManager.GetUserId(User) && a.CheckIn.Date == date);

            return attendanceToday != null;
            */
            return false;
        }
    }
}
