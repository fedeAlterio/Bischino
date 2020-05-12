using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Model;
using Skribble.Model;

namespace Bischino.Skribble
{
    public class SkribbleHandler : ISkribbleHandler
    {
        private RoomsCollection _roomsCollection = new RoomsCollection();


        public SkribbleHandler()
        {
            _roomsCollection.Add(new RoomManager());
        }



        public void UpdateDraw(RoomQuery<IEnumerable<Line>> drawUpdatedQuery)
        {
            var roomManager = GetRoomManager(drawUpdatedQuery);
            roomManager.NotifyPlayerJoined("player1");
            roomManager.NotifyPlayerJoined("player2");

            roomManager.AddDrawUpdate(drawUpdatedQuery.PlayerName, drawUpdatedQuery.Data);
        }



        public Task<IEnumerable<Line>> GetDrawUpdate(RoomQuery roomQuery)
        {
            var roomManager = GetRoomManager(roomQuery);
            var ret = roomManager.GetDrawUpdate(roomQuery.PlayerName);

            return ret;
        }



        private RoomManager GetRoomManager(RoomQuery roomQuery) => _roomsCollection.Get("Room");
    }
}
