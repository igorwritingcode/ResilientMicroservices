using System;
using System.Net;
using System.Runtime.Serialization;

namespace Users.Api.Infrastructure.Exceptions
{
    public class CustomException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public CustomException(string message)
            : base(message) { }

        public CustomException(string message, Exception innerException)
            : base(message, innerException) { }

        public CustomException(string errorMessage,
                              HttpStatusCode statusCode)
        : base(errorMessage)
        {
            HttpStatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        protected CustomException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}
