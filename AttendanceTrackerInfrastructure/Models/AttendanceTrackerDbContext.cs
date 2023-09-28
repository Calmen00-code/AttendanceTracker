using Microsoft.EntityFrameworkCore;

namespace AttendanceTrackerInfrastructure.Models
{
    public class AttendanceTrackerDbContext: DbContext
    {
        public AttendanceTrackerDbContext(DbContextOptions<AttendanceTrackerDbContext> options) 
            : base(options) 
        { 
        
        }

        public DbSet<Admin> admins { get; set; }

        public DbSet<Staff> staffs { get; set; }

        public DbSet<WorkdayRecord> workdays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StaffConfiguration());
            modelBuilder.ApplyConfiguration(new AdminConfiguration());
            modelBuilder.ApplyConfiguration(new WorkdayRecordConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        }
    }
}
