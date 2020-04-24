using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Model
{
    public class RoomQuery
    {
        public string RoomName { get; set; }
        public string PlayerName { get; set; }
        public int? RoomNumber { get; set; }
    }

    public class RoomQuery<T> : RoomQuery
    {
        public T Data { get; set; }
    }
}
