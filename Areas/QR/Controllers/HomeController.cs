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

namespace AttendanceTracker.Controllers
{
    [Area("QR")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IDistributedCache _cache;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager, IDistributedCache cache)
        {
            _logger = logger;
            _signInManager = signInManager;
            _cache = cache;
        }

        public IActionResult Authentication(string token)
        {
            // if (string.IsNullOrEmpty(token) || !IsTokenValid(token))
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            AuthenticationVM authenticationVM = new ()
            {
                Token = token
            };

            return View(authenticationVM);
        }

        public IActionResult OnSuccessRecord()
        {
            return View();
        }

        public IActionResult OnFailRecord()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authentication(AuthenticationVM model)
        {
            // Check if token has expired
            if (!IsTokenValid(model.Token))
            {
                TempData["error"] = "Token has expired. Please rescan the QR code.";
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Clear the token after successful authentication
                    HttpContext.Session.Clear();

                    return RedirectToAction("OnSuccessRecord", "Home", new { area = "QR" });
                }
                else
                {
                    TempData["error"] = "Invalid username and password. Please try again.";
                    // use TempData keep here to persist the error message across redirects
                    // as TempData["error"] only lasts for one request. Redirection will clear the TempData.
                    TempData.Keep("error");
                }
            }
            
            // If we got this far, something failed, redisplay form
            return RedirectToAction("OnFailRecord", "Home", new { area = "QR", token=HttpContext.Session.GetString(SD.GUID_SESSION) });
        }

        private bool IsTokenValid(string token)
        {
            string sessionToken = HttpContext.Session.GetString(SD.GUID_SESSION);
            return (!string.IsNullOrEmpty(token)) && (HttpContext.Session.GetString(SD.GUID_SESSION) == token);
        }

        public IActionResult Index()
        {
            // Generate a unique GUID token for the QR code which expires when the user check in or check out
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SD.GUID_SESSION)))
            {
                string guidToken = Guid.NewGuid().ToString();
                HttpContext.Session.SetString(SD.GUID_SESSION, guidToken);
            }

            // Define and embed token into authentication page URL
            string authenticationUrl = 
                $"{Url.Action("Authentication", "Home", new { area = "QR" }, Request.Scheme)}?token={HttpContext.Session.GetString(SD.GUID_SESSION)}";

            // Define the authentication page URL
            //string authenticationUrl = Url.Action("Authentication", "Home", new { area = "QR" }, Request.Scheme);

            // QR Code Generation on Page Load
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeInfo = qrGenerator.CreateQrCode(authenticationUrl, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeInfo);
            byte[] qrCodeBytes = qrCode.GetGraphic(60);
            
            // Convert QR Code to Base64 URI
            string qrUri = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
            ViewBag.QrCodeUri = qrUri;

            return View();
        }
    }
}
