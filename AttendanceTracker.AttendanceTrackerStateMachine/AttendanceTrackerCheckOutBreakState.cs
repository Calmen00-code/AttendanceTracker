// Concrete class definition for check out during a break

using System.Security.Claims;
using AttendanceTracker.Utility;

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckOutBreakState : IAttendanceTrackerState
    {
        public bool RecordAttendance(AttendanceTrackerStateContext context, ClaimsPrincipal user)
        {
            Console.WriteLine("User check in successfully");
            context.TransitionTo(new AttendanceTrackerCheckInBreakState());
            return true;
        }

        public SD.AttendanceState GetStateIdentifier()
        {
            return SD.AttendanceState.CheckOutBreak;
        }
    }
}