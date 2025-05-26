// Concrete class definition for check out during a break

using AttendanceTracker.Utility;

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckOutBreakState : IAttendanceTrackerState
    {
        private const string StateIdentifier = "AttendanceTrackerCheckOutBreakState";

        public AttendanceTrackerCheckOutBreakState()
        {
            Console.WriteLine("Only enable check in break button");
        }

        public void CheckIn(AttendanceTrackerStateContext context)
        {
            Console.WriteLine("User check in successfully");
            context.TransitionTo(new AttendanceTrackerCheckInBreakState());
        }

        public void CheckOut(AttendanceTrackerStateContext context)
        {
            throw new NotImplementedException("User already checked in.");
        }

        public SD.AttendanceState GetStateIdentifier()
        {
            return SD.AttendanceState.CheckOutBreak;
        }
    }
}