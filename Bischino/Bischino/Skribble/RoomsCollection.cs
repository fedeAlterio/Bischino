using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Controllers;
using Bischino.Extensions;

namespace Bischino.Skribble
{
    public class RoomsCollection
    {
        private readonly IList<RoomManager> _roomManagers = new List<RoomManager>();
        private readonly object _lock = new object();
        private readonly IList<int> Numbers;
        private const int TotRooms = 99999;



        public RoomsCollection()
        {
            Numbers = Enumerable.Range(0, TotRooms).ToList();
            Numbers.Shuffle();
        }



        public RoomManager Get(int roomNumber)
        {
            lock (_lock)
            {
                var ret = _roomManagers.FirstOrDefault(rm => rm.Room.RoomNumber == roomNumber);
                if (ret is null)
                    throw new ValidationException($"The room {roomNumber} does not exist");
                return ret;
            }
        }

        public RoomManager Get(string roomName)
        {
            lock (_lock)
            {
                var ret = _roomManagers.FirstOrDefault(rm => rm.Room.Name == roomName);
                if (ret is null)
                    throw new ValidationException($"The room {roomName} does not exist");
                return ret;
            }
        }



        public void Add(RoomManager roomManager)
        {
            lock (_lock)
            {
                if (_roomManagers.Any(rm => rm.Room.Name == roomManager.Room.Name))
                    throw new ValidationException("There is already a room with this name");

                var roomNumber = Numbers[0];
                Numbers.RemoveAt(0);
                roomManager.Room.RoomNumber = roomNumber;

                _roomManagers.Add(roomManager);
            }
        }


        public void Remove(RoomManager roomManager)
        {
            lock (_lock)
            {
                if (!_roomManagers.Remove(roomManager))
                    return;
                if (roomManager.Room.RoomNumber is null)
                    throw new Exception("Room number cannot be null");

                Numbers.Add(roomManager.Room.RoomNumber.Value);
            }
        }



        public RoomManager First()
        {
            lock (_lock)
            {
                return _roomManagers[0];
            }
        }



        public long Count => _roomManagers.Count;
    }
}
