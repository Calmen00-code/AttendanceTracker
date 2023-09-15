using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClasses
{
    /***
     * Universal class that is used by all the application to represent 
     * their HTTP status. We can find the definition for each of the code 
     * here: https://developer.mozilla.org/en-US/docs/Web/HTTP/Status
     */
    public class HttpResponseStatus
    {
        // Successful response
        public static readonly int OK = 200;
        public static readonly int CREATED = 201;
        public static readonly int ACCEPTED = 202;

        // Client error responses
        public static readonly int NOT_FOUND = 404;

        // Server error responses
        public static readonly int INTERNAL_SERVER_ERROR = 500;
    }
}
