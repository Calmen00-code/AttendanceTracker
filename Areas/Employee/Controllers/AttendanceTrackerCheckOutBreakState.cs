// Concrete class definition for check out during a break

using AttendanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AttendanceTracker.Controllers
{
    public class AttendanceTrackerCheckOutBreakState : IAttendanceTrackerState
    {
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
    }
}