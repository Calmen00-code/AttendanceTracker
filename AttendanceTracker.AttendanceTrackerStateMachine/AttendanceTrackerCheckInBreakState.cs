// Concrete class definition for check in after a break

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckInBreakState : IAttendanceTrackerState
    {
        private const string StateIdentifier = "AttendanceTrackerCheckInBreakState";

        public AttendanceTrackerCheckInBreakState()
        {
            Console.WriteLine("Only enable check out of the day button");
        }

        public void CheckIn(AttendanceTrackerStateContext context)
        {
            Console.WriteLine("User check in after lunch successfully");
            context.TransitionTo(new AttendanceTrackerCheckOutState());
        }

        public void CheckOut(AttendanceTrackerStateContext context)
        {
            throw new NotImplementedException("User already checked in.");
        }

        public string GetStateIdentifier()
        {
            return StateIdentifier;
        }
    }
}