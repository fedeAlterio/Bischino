using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Base.Controllers
{
    public class ServerResponse
    {
        public static ValuedResponse Error => new ValuedResponse { Message = "An error occurred" };
        public static ValuedResponse Unauthorized => new ValuedResponse { Message = "Unauthorized" };
        public string Message { get; set; }
    }

    public class ValuedResponse : ServerResponse
    {
        public ValuedResponse(object value = null)
        {
            if(value is null)
                Value = new Object();
            Value = value;
        }
        public object Value { get; set; }
    }
}
