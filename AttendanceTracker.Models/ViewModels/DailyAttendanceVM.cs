using System.ComponentModel.DataAnnotations;
using AttendanceTracker.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AttendanceTracker.Models.ViewModels
{
    public class DailyAttendanceVM
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
    }
}