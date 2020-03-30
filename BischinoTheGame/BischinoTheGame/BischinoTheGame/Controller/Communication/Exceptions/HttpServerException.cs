using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BischinoTheGame.Controller.Communication.Exceptions
{
    public class HttpServerException : ServerException
    {
        public HttpStatusCode StatusCode { get; }

        public HttpServerException(HttpStatusCode statusCode, string message = "") : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpServerException(HttpStatusCode statusCode, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
