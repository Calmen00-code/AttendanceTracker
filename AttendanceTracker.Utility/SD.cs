namespace AttendanceTracker.Utility
{
    // Static details
    public class SD
    {
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_EMPLOYEE = "Employee";

        public const string GUID_SESSION = "GuidSession";

        public enum AttendanceState
        {
            CheckIn,
            CheckOutBreak,
            CheckInBreak,
            CheckOut
        }
    }
}
