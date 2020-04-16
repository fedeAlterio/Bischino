using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BischinoTheGame.Controller.Communication.Exceptions
{
    /// <summary>
    /// <see cref="ServerException"/> that encapsulates an Http Status Code
    /// </summary>
    public class HttpServerException : ServerException
    {
        /// <summary>
        /// Http status code returned from the server.
        /// </summary>
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
