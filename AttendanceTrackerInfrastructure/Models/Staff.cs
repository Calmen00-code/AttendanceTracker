namespace AttendanceTrackerInfrastructure.Models
{
    public class Staff
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Password { get; set; }

        public string? Department { get; set; }

        public List<WorkdayRecord>? Workdays { get; set; }
    }
}
