// Concrete class definition to check out of the day

using AttendanceTracker.Utility;

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckOutState : IAttendanceTrackerState
    {
        public void RecordAttendance(AttendanceTrackerStateContext context)
        {
            // TODO: API to check out for the day
            // code start here
            // ...
            // code end here
            Console.WriteLine("User checkout for the day successfully.");
        }

        public SD.AttendanceState GetStateIdentifier()
        { 
            return SD.AttendanceState.CheckOut; 
        }
    }
}