// Concrete class definition to check out of the day

using System.Security.Claims;
using AttendanceTracker.Utility;

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerCheckOutState : IAttendanceTrackerState
    {
        public bool RecordAttendance(AttendanceTrackerStateContext context, ClaimsPrincipal user)
        {
            // TODO: API to check out for the day
            // code start here
            // ...
            // code end here
            Console.WriteLine("User checkout for the day successfully.");
            return true;
        }

        public SD.AttendanceState GetStateIdentifier()
        { 
            return SD.AttendanceState.CheckOut; 
        }
    }
}