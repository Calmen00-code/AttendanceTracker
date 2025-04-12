using System.ComponentModel.DataAnnotations;

namespace AttendanceTracker.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
