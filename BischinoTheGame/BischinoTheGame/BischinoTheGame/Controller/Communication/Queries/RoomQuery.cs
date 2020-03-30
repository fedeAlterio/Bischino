using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.Controller.Communication.Queries
{
    public class RoomQuery
    {
        public string RoomName { get; set; }
        public string PlayerName { get; set; }
    }

    public class RoomQuery<T> : RoomQuery
    {
        public T Data { get; set; }
    }
}
