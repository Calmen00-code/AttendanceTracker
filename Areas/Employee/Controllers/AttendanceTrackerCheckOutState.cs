// Concrete class definition to check out of the day

using AttendanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AttendanceTracker.Controllers
{
    public class AttendanceTrackerCheckOutState : IAttendanceTrackerState
    {
        public AttendanceTrackerCheckOutState()
        {
        }

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
    }
}