using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClasses
{
    public class WorkdayRecordAPI
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckOut { get; set; }

        public string StaffName { get; set; } 

        public StaffAPI Staff { get; set; }

    }
}
