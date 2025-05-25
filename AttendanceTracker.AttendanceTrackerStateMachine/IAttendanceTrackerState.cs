/**
 * @brief Interface for defining state behavior in an attendance tracking system.
 *
 * This class serves as the base for different attendance states (e.g., Check-In, Check-Out).
 * It provides a common interface for managing state-specific behavior in the system.
 *
 * Refer to what each state does in their respective classes:
 * 1. AttendanceTrackerCheckInState.cs: Handles user check-in for the day.
 * 2. AttendanceTrackerCheckOutBreakState.cs: Handles user check-out for lunch break.
 * 3. AttendanceTrackerCheckInBreakState.cs: Handles user check-in after lunch break.
 * 4. AttendanceTrackerCheckOutState.cs: Handles user check-out for the day.
 *
 */

namespace AttendanceTracker.AttendanceTrackerStateMachine
{
    public interface IAttendanceTrackerState
    {
        public void CheckIn(AttendanceTrackerStateContext context);
        public void CheckOut(AttendanceTrackerStateContext context);
    }
}