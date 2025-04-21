using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceTracker.Models;

namespace AttendanceTracker.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}
