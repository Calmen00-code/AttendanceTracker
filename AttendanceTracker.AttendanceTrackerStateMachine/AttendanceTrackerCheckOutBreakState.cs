// Concrete class definition for check out during a break

using AttendanceTracker.Utility;

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckOutBreakState : IAttendanceTrackerState
    {
        public void RecordAttendance(AttendanceTrackerStateContext context)
        {
            Console.WriteLine("User check in successfully");
            context.TransitionTo(new AttendanceTrackerCheckInBreakState());
        }

        public SD.AttendanceState GetStateIdentifier()
        {
            return SD.AttendanceState.CheckOutBreak;
        }
    }
}