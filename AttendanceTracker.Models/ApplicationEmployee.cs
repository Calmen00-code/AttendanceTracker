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
    }
}
