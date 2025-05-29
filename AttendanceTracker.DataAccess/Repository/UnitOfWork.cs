
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AttendanceTracker.DataAccess.Data;
using AttendanceTracker.Models;

namespace AttendanceTracker.DataAccess.Repository.IRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IAttendanceRepository Attendance { get; private set; }
        public IDailyAttendanceRecordRepository DailyAttendanceRecord { get; private set; }
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Attendance = new AttendanceRepository(_db);
            DailyAttendanceRecord = new DailyAttendanceRecordRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}



