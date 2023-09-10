using System.ComponentModel.DataAnnotations;

namespace AttendanceTrackerInfrastructure.Models
{
    public class Staff
    {
        [Key]
        public required string Name { get; set; }

        public required string Password { get; set; }

        public string? Department { get; set; }

        public ICollection<WorkdayRecord>? Workdays { get; set; }
    }
}
