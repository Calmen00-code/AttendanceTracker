using System.ComponentModel.DataAnnotations;
using AttendanceTracker.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AttendanceTracker.Models.ViewModels
{
    public class AttendanceVM
    {
        public string EmployeeId { get; set; }
        public string Email { get; set; }

        public string Date { get; set; }

        public double TotalWorkingHours { get; set; }
    }
}