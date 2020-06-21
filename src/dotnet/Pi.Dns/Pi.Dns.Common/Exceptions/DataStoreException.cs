using System;
using System.Net;

namespace Pi.Dns.Common.Exceptions
{
    public class DatastoreException : Exception
    {
        public DatastoreException()
        {
        }

        public DatastoreException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public DatastoreException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }

        public DatastoreException(HttpStatusCode httpStatusCode, string message, TimeSpan retryAfter, Exception innerException) : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
            RetryAfter = retryAfter;
        }

        public HttpStatusCode HttpStatusCode { get; set; }
        public TimeSpan RetryAfter { get; }
    }
}
