using Microsoft.EntityFrameworkCore;

namespace AttendanceTrackerInfrastructure.Models
{
    public class AdminContext: DbContext
    {
        public AdminContext(DbContextOptions<AdminContext> options)
            : base(options)
        {

        }

        public DbSet<Admin> admins { get; set; } = null!;
    }
}
