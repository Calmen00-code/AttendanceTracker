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

namespace AttendanceTracker.Controllers
{
    [Area("QR")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IDistributedCache _cache;
        private readonly IHubContext<RefreshHub> _hubContext;

        public HomeController(
            ILogger<HomeController> logger,
            SignInManager<IdentityUser> signInManager,
            IDistributedCache cache,
            IHubContext<RefreshHub> hubContext)
        {
            _logger = logger;
            _signInManager = signInManager;
            _cache = cache;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            // Generate and initialize unique GUID token for the QR code which expires when the user check in or check out
            // Used at the first time the app is loaded
            if (string.IsNullOrEmpty(_cache.GetString(SD.GUID_SESSION)))
            {
                string guidToken = GenerateNewToken();

                // TODO: might need to add a lock here to ensure thread safety
                _cache.SetString(SD.GUID_SESSION, guidToken);
            }

            GenerateQRCode();

            return View();
        }

        public IActionResult Authentication(string token)
        {
            if (string.IsNullOrEmpty(token) || !IsTokenValid(token))
            {
                return UnauthorizedAction("Token has expired. Please rescan the QR code.");
            }

            AuthenticationVM authenticationVM = new ()
            {
                Token = token
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
                    // TODO: might need to add a lock here to ensure thread safety

                    _cache.SetString(SD.GUID_SESSION, GenerateNewToken());

                    GenerateQRCode();

                    // force refresh the page on all clients
                    await _hubContext.Clients.All.SendAsync("RefreshPage");

                    return RedirectToAction("OnSuccessRecord", "Home", new { area = "QR" });
                }
                else
                {
                    return RedirectToAction("UnauthorizedAction", "Home",
                        new { area = "QR", message = "Incorrect username and password! Please rescan the QR code" });
                }
            }
            
            // If we got this far, something failed, redisplay form
            return RedirectToAction("UnauthorizedAction", "Home",
                new { area = "QR", message = "Something went wrong, please rescan QR and try again..." });
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
    }
}
