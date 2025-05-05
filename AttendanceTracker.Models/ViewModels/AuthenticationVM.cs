using System.ComponentModel.DataAnnotations;

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
    }
}