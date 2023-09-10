using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClasses
{
    public class StaffAPI
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public string? Department { get; set; }

        public ICollection<WorkdayRecordAPI>? Workdays { get; set; }

    }
}
