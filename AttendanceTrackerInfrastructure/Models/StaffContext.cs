using Microsoft.EntityFrameworkCore;

namespace AttendanceTrackerInfrastructure.Models
{
    public class StaffContext: DbContext
    {
        public StaffContext(DbContextOptions<StaffContext> options) 
            : base(options) 
        { 

        }

        public DbSet<Staff> staffs { get; set; } = null!;
    }
}
