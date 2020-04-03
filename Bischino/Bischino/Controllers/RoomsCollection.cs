using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Controllers;
using Bischino.Base.Model;
using Bischino.Bischino;
using Bischino.Controllers.Queries;

namespace Bischino.Controllers
{
    public class RoomsCollection
    {
        private readonly IList<RoomManager> _roomManagers = new List<RoomManager>();
        private readonly object _lock = new object();


        public RoomManager Get(string roomName)
        {
            lock (_lock)
            {
                return _roomManagers.FirstOrDefault(rm => rm.Room.Name == roomName);
            }
        }



        public void Add(RoomManager roomManager)
        {
            lock (_lock)
            {
                if(_roomManagers.Any(rm => rm.Room.Name == roomManager.Room.Name))
                    throw new ValidationException("There is already a room with this name");

                _roomManagers.Add(roomManager);
            }
        }


        public void Remove(RoomManager roomManager)
        {
            lock (_lock)
            {
                _roomManagers.Remove(roomManager);
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
                    where !rm.IsGameStarted
                    select rm.Room)
                    .Skip(roomSearchQuery.Options.Skip)
                    .Take(roomSearchQuery.Options.Limit); 

                var ret =  roomManagers.ToList();
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
    }
}
