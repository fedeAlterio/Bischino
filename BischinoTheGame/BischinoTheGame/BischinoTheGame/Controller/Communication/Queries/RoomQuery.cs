using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.Controller.Communication.Queries
{
    /// <summary>
    /// Contains all the data to reference a unique player in a room.
    /// </summary>
    public class RoomQuery
    {
        /// <summary>
        /// The user-friendly name of a room. This field is unique
        /// </summary>
        public string RoomName { get; set; }


        /// <summary>
        /// The username of a player in the room referenced by <see cref="RoomName"/>
        /// </summary>
        public string PlayerName { get; set; }



        public int? RoomNumber { get; set; }
    }


    ///<inheritdoc cref="RoomQuery"/>
    /// <summary>
    /// This query encapsulates an object of type T.
    /// </summary>
    /// <typeparam name="T">Type of the extra data contained in the query</typeparam>
    public class RoomQuery<T> : RoomQuery
    {
        public T Data { get; set; }
    }
}
