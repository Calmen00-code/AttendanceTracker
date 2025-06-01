using AttendanceTracker.DataAccess.Data;
using AttendanceTracker.Models;

namespace AttendanceTracker.DataAccess.Repository.IRepository
{
    public class ApplicationEmployeeRepository : Repository<ApplicationEmployee>, IApplicationEmployeeRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationEmployeeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationEmployee employee)
        {
            _db.ApplicationEmployees.Update(employee);
            _db.SaveChanges();
        }
    }
}