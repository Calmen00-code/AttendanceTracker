using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AttendanceTracker.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public double TotalWorkingHours { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [ValidateNever]
        public Employee Employee { get; set; }
    }
}
