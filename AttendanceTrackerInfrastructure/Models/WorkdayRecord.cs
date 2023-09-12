using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceTrackerInfrastructure.Models
{
    /***
     * In this class, we are introducing Composite Key into the database
     * @problem: Initially we were using StaffName as the only ForeignKey, but
     *           adding more than one Date to the same StaffName will violate the 
     *           PRIMARY KEY constraint in the Database.
     *
     * @expect:  We should allow more than one Date being added to the same StaffName
     *           because a Staff is not only working for one day, it is more than a single day. 
     *           If that is the case, we are defeating the purpose of WorkdayRecord here.
     *           
     * @fix:     Introduced Composite Key here by having Date as the order = 0, and 
     *           StaffName order = 1. This order will later be reflected in the database.
     *           If order is not specified, SQL will try to order them in alphabetical order
     */
    public class WorkdayRecord
    {
        [DataType(DataType.Date)]
        [Key]
        [Column(Order = 0)]
        public DateTime Date { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Staff")]
        public string StaffName { get; set; } // Foreign Key to Staff.Name

        [DataType(DataType.Time)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckOut { get; set; }

        public Staff Staff { get; set; } // Navigation Property to Staff

        // DateTime will store date and time together
        // we do not need that here, we will take care of time in CheckIn and CheckOut
        // therefore this function is created to handle the issue, thus not mapped to database
        [NotMapped]
        public string DateSpecific
        {
            get
            {
                return Date.ToString("HH:mm");
            }

            set
            {
                Date = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }


        // CheckInTime is not mapped to the database, we do this to ease our 
        // time calculation later on, because the default time captured is in 
        // this format: HH:mm:ss. We do not need to have seconds considered
        [NotMapped]
        public string CheckInTime
        {
            get
            {
                return CheckIn.ToString("HH:mm");
            }

            set
            {
                CheckIn = DateTime.ParseExact(value, "HH::mm", null);
            }
        }

        // CheckOutTime is not mapped to the database, we do this to ease our 
        // time calculation later on, because the default time captured is in 
        // this format: HH:mm:ss. We do not need to have seconds considered
        [NotMapped]
        public string CheckOutTime
        {
            get
            {
                return CheckOut.ToString("HH:mm");
            }

            set
            {
                CheckOut = DateTime.ParseExact(value, "HH::mm", null);
            }
        }
    }
}
