using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceTracker.Models
{
    public class ApplicationEmployee : IdentityUser
    {
        [ValidateNever]
        public ICollection<Attendance> Attendances { get; set; }

        // study if we need to save QR into DB or not or we can just store it in static file
    }
}
