/**
 * @brief Manages state transitions for the attendance tracker system.
 *
 * This class maintains the current state of the attendance tracker and allows 
 * transitions between different attendance states (e.g., Check-In, Check-Out).
 *
 * The states follow the following order:
 * 1. AttendanceTrackerCheckInState (User checks in for the day)
 * 2. AttendanceTrackerCheckOutBreakState (User check out for lunch break)
 * 3. AttendanceTrackerCheckInBreakState (User checks in after lunch break)
 * 4. AttendanceTrackerCheckOutState (User checks out for the day)
 *
 */

using AttendanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AttendanceTracker.Controllers
{
    [Area("Employee")]
    public class AttendanceTrackerStateContext
    {
        // A reference to the current state of the attendance tracker
        private IAttendanceTrackerState _state;

        public AttendanceTrackerStateContext(IAttendanceTrackerState initialState)
        {
            // Always start with AttendanceTrackerCheckInState
            _state = initialState;
        }

        public void TransitionTo(IAttendanceTrackerState newState)
        {
            _state = newState;
        }

        public void RequestCheckIn()
        {
            _state.CheckIn(this);
        }

        public void RequestCheckOut()
        {
            _state.CheckOut(this);
        }
    }
}