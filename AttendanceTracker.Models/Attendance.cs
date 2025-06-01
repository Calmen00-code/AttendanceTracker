using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AttendanceTracker.Models
{
    public class Attendance
    {
        [Key]
        public string Id { get; set; }

        public double TotalWorkingHours { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [ValidateNever]
        public ApplicationEmployee Employee { get; set; }
    }
}
