

using System.Security.Claims;

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
namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public class AttendanceTrackerStateContext
    {
        // A reference to the current state of the attendance tracker
        private IAttendanceTrackerState _state;

        public AttendanceTrackerStateContext()
        {
            // Always start with AttendanceTrackerCheckInState
            _state = new AttendanceTrackerCheckInState();
        }

        public void TransitionTo(IAttendanceTrackerState newState)
        {
            _state = newState;
        }

        public bool RequestAttendanceRecordAction(ClaimsPrincipal user)
        {
            bool status = _state.RecordAttendance(this, user);
            return status;
        }

        public IAttendanceTrackerState GetCurrentState()
        {
            return _state;
        }
    }
}