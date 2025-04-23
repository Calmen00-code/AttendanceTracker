using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AttendanceTracker.DataAccess.Data;
using AttendanceTracker.Models;

namespace AttendanceTracker.DataAccess.Repository.IRepository
{
    public class AttendanceTracker : Repository<Attendance>, IAttendanceRepository
    {
        private readonly ApplicationDbContext _db;
        public AttendanceTracker(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Attendance attendance)
        {
            _db.Attendances.Update(attendance);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}