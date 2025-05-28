// Concrete class definition for check in after a break

using System.Security.Claims;
using AttendanceTracker.Utility;

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckInBreakState : IAttendanceTrackerState
    {
        public bool RecordAttendance(AttendanceTrackerStateContext context, ClaimsPrincipal user)
        {
            Console.WriteLine("User check in after lunch successfully");
            context.TransitionTo(new AttendanceTrackerCheckOutState());
            return true;
        }

        public SD.AttendanceState GetStateIdentifier()
        {
            return SD.AttendanceState.CheckInBreak;
        }
    }
}