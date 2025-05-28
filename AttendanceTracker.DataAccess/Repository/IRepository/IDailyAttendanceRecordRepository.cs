using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AttendanceTracker.Models;

namespace AttendanceTracker.DataAccess.Repository.IRepository
{
    public interface IDailyAttendanceRecordRepository : IRepository<DailyAttendanceRecord>
    {
        void Update(DailyAttendanceRecord dailyAttendanceRecord);
    }
}