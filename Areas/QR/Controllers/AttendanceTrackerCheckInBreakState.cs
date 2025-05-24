// Concrete class definition for check in after a break

using AttendanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AttendanceTracker.Controllers
{
    public class AttendanceTrackerCheckInBreakState : IAttendanceTrackerState
    {
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
    }
}