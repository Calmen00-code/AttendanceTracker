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

namespace AttendanceTracker.Controllers
{
    [Area("QR")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult Authentication(string token)
        {
            // if (string.IsNullOrEmpty(token) || !IsTokenValid(token))
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }
            return View(new AuthenticationVM());
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
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Check in or check out logic here
                    // You can use the guidToken to identify the user session
                    // and perform the check-in/check-out operation

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

        // private bool IsTokenValid(string token)
        // {
        //     var cache = ConnectionMultiplexer.Connect("localhost").GetDatabase();
        //     string storedToken = cache.StringGet($"token:{token}");

        //     return !string.IsNullOrEmpty(storedToken); // Only allow valid tokens
        // }

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
