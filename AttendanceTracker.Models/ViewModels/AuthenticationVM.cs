using System.ComponentModel.DataAnnotations;
using AttendanceTracker.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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

        [ValidateNever]
        public string EmployeeId { get; set; }

        [ValidateNever]
        public bool IsCheckIn { get; set; }

        public string Token { get; set; }
    }
}