// Concrete class definition for check in after a break

using AttendanceTracker.Utility;

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckInBreakState : IAttendanceTrackerState
    {
        public void RecordAttendance(AttendanceTrackerStateContext context)
        {
            Console.WriteLine("User check in after lunch successfully");
            context.TransitionTo(new AttendanceTrackerCheckOutState());
        }

        public SD.AttendanceState GetStateIdentifier()
        {
            return SD.AttendanceState.CheckInBreak;
        }
    }
}