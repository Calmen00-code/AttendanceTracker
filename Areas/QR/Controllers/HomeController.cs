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

namespace AttendanceTracker.Controllers
{
    [Area("QR")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;

        private string guidToken = string.Empty;

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
                    guidToken = string.Empty;

                    return RedirectToAction("OnSuccessRecord", "Home", new { area = "QR" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            
            // If we got this far, something failed, redisplay form
            return View(guidToken);
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
            if (string.IsNullOrEmpty(guidToken))
            {
                guidToken = Guid.NewGuid().ToString();
            }

            // Define and embed token into authentication page URL
            string authenticationUrl = 
                $"{Url.Action("Authentication", "Home", new { area = "QR" }, Request.Scheme)}?token={guidToken}";

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
