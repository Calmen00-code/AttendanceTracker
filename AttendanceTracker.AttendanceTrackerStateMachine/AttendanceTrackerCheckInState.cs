// Concrete class definition for the first check in of the day

using AttendanceTracker.Utility;
using AttendanceTracker.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using AttendanceTracker.Models;
using System.Security.Claims;

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckInState : IAttendanceTrackerState
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceTrackerCheckInState(SignInManager<IdentityUser> signInManager, IUnitOfWork unitOfWork)
        {
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        public bool RecordAttendance(AttendanceTrackerStateContext context, ClaimsPrincipal user)
        {
            // First time check in for the day
            string userId = _signInManager.UserManager.GetUserId(user);
            string attendanceId = DateTime.Now.ToString("yyyyMMdd") + "_" + userId;

            // User has not checked in for the day yet, so add a new record
            // FIXME: We have updated a new DB schema, this has to be updated as it is no longer valid
            _unitOfWork.DailyAttendanceRecords.Add(new DailyAttendanceRecord
            {
                Id = attendanceId,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.MinValue,
                TotalWorkingHours = 0,
                EmployeeId = _signInManager.UserManager.GetUserId(User)
            });

            _unitOfWork.Save();
            context.TransitionTo(new AttendanceTrackerCheckOutBreakState());
            return true;
        }

        public SD.AttendanceState GetStateIdentifier()
        {
            return SD.AttendanceState.CheckIn;
        }
    }
}