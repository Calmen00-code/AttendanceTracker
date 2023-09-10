using System.ComponentModel.DataAnnotations;

namespace AttendanceTrackerInfrastructure.Models
{
    public class WorkdayRecord
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckOut { get; set; }
    }
}
