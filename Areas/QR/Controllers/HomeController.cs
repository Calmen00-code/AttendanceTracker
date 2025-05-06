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

namespace AttendanceTracker.Controllers
{
    [Area("QR")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string guidToken = string.Empty;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Authentication(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }
            return View(new AuthenticationVM());
        }

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
