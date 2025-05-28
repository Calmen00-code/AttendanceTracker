using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AttendanceTracker.DataAccess.Data;
using AttendanceTracker.Models;

namespace AttendanceTracker.DataAccess.Repository.IRepository
{
    public class DailyAttendanceRecordRepository : Repository<DailyAttendanceRecord>, IDailyAttendanceRecordRepository
    {
        private readonly ApplicationDbContext _db;

        public DailyAttendanceRecordRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(DailyAttendanceRecord dailyAttendanceRecord)
        {
            _db.DailyAttendanceRecords.Update(dailyAttendanceRecord);
            _db.SaveChanges();
        }
    }
}