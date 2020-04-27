using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Controllers;
using Bischino.Base.Model;
using Bischino.Extensions;

namespace Bischino.Model
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
                if( ret is null)
                    throw new ValidationException($"The room {roomName} does not exist");
                return ret;
            }
        }



        public void Add(RoomManager roomManager)
        {
            lock (_lock)
            {
                if(_roomManagers.Any(rm => rm.Room.Name == roomManager.Room.Name))
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

        
        public IList<Room> GetRooms(RoomSearchQuery roomSearchQuery)
        {
            lock (_lock)
            {
                var roomManagers = (from rm in _roomManagers.Except
                        (
                            from rm in _roomManagers
                            from p1 in ModelBase.PropertiesDictionary(rm.Room)
                            join p2 in ModelBase.PropertiesDictionary(roomSearchQuery.Model) on p1.Key equals p2.Key
                            where !p1.Value.Equals(p2.Value)
                            select rm
                        )
                        where !rm.IsGameStarted && !rm.Room.IsPrivate.Value
                        let isValid = rm.Room.NotBotPlayers.Any() || rm.Room.CreationDate > DateTime.Now.Subtract(TimeSpan.FromHours(1))
                        select new {RoomManger = rm, IsValid = isValid})
                    .Skip(roomSearchQuery.Options.Skip)
                    .ToList();
                var validRoomsManagers = (from rm in roomManagers where rm.IsValid select rm).Take(roomSearchQuery.Options.Limit);
                foreach (var rm in (from rm in roomManagers where !rm.IsValid select rm).Take(10))
                    _roomManagers.Remove(rm.RoomManger);


                var ret =  validRoomsManagers.Select(rm => rm.RoomManger.Room).ToList();
                return ret;
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
