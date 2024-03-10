using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClasses
{
    public class APILinks
    {
        // Infrastructure Layer
        public static readonly string API_URL_INFRASTRUCTURE_DEVELOPMENT = "https://localhost:7102/api/AttendanceTrackerInfrastructure/";
        public static readonly string API_URL_INFRASTRUCTURE_DEPLOYMENT = "https://attendancetrackerinfrastructure.azurewebsites.net/api/AttendanceTrackerInfrastructure/";

        // Application Layer
        public static readonly string API_URL_APPLICATION_DEVELOPMENT = "https://localhost:7178/api/AttendanceTrackerApplication/";
        public static readonly string API_URL_APPLICATION_DEPLOYMENT = "https://attendancetrackerapplication.azurewebsites.net/api/AttendanceTrackerApplication/";
    }
}
