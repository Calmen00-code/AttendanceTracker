using System.ComponentModel.DataAnnotations;

namespace AttendanceTrackerInfrastructure.Models
{
    public class Department
    {
        [Key]
        public string? Name { get; set; }
    }
}
