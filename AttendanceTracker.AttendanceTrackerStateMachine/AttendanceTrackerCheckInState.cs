// Concrete class definition for the first check in of the day

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckInState : IAttendanceTrackerState
    {
        public void CheckIn(AttendanceTrackerStateContext context)
        {
            throw new NotImplementedException("User already checked in.");
        }

        public void CheckOut(AttendanceTrackerStateContext context)
        {
            // TODO: API to check out
            // code start here
            // ...
            // code end here
            Console.WriteLine("User checked out for lunch break successfully.");
            context.TransitionTo(new AttendanceTrackerCheckOutBreakState());
        }
    }
}