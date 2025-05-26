// Concrete class definition to check out of the day

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckOutState : IAttendanceTrackerState
    {
        private const string StateIdentifier = "AttendanceTrackerCheckOutState";

        public void CheckIn(AttendanceTrackerStateContext context)
        {
            throw new NotImplementedException("User already checked in after lunch break.");
        }

        public void CheckOut(AttendanceTrackerStateContext context)
        {
            // TODO: API to check out for the day
            // code start here
            // ...
            // code end here
            Console.WriteLine("User checkout for the day successfully.");
        }

        public string GetStateIdentifier()
        { 
            return StateIdentifier; 
        }
    }
}