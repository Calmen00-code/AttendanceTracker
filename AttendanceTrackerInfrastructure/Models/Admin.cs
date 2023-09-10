using System.ComponentModel.DataAnnotations;

namespace AttendanceTrackerInfrastructure.Models
{
    public class Admin
    {
        [Key]
        public required string Name { get; set; }

        public required string Password { get; set; }
    }
}
