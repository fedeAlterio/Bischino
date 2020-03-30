using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.Controller.Communication
{
    public class ResponseHeader
    {
        public string Message { get; set; }
    }

    public class ResponseHeader<T> : ResponseHeader
    {
        public T Value { get; set; }
    }
}
