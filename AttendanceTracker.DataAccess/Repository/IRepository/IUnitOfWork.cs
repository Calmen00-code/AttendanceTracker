using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceTracker.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IAttendanceRepository Attendance { get; }

        void Save();
    }
}