// Concrete class definition for the first check in of the day

using AttendanceTracker.Utility;

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckInState : IAttendanceTrackerState
    {
        public void RecordAttendance(AttendanceTrackerStateContext context)
        {
            // TODO: API to check out
            // code start here
            // ...
            // code end here
            Console.WriteLine("User checked out for lunch break successfully.");
            context.TransitionTo(new AttendanceTrackerCheckOutBreakState());
        }

        public SD.AttendanceState GetStateIdentifier()
        {
            return SD.AttendanceState.CheckIn;
        }
    }
}