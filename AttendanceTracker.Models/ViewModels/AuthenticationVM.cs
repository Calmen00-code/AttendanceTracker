using System.ComponentModel.DataAnnotations;
using AttendanceTracker.Controllers;

namespace AttendanceTracker.Models.ViewModels
{
    public class AuthenticationVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Token { get; set; }

        public bool IsCheckIn { get; set; }

        public IAttendanceTrackerState AttendanceTrackerState { get; set; }
    }
}