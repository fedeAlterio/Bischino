using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Model;

namespace Bischino.Model
{
    public class JoinedPlayerInfo
    {
        public string Name { get; }
        public string RoomName { get; }

        public JoinedPlayerInfo(string name, string roomName)
        {
            (Name, RoomName) = (name, roomName);
        }

        public override bool Equals(object obj)
        {
            if (obj is JoinedPlayerInfo joinedPlayer)
                return Name == joinedPlayer.Name && RoomName == joinedPlayer.RoomName;
            return false;
        }
        
        public override int GetHashCode()
        {
            return (Name?.GetHashCode() ?? 0) ^ (RoomName?.GetHashCode() ?? 0);
        }

        public static JoinedPlayerInfo FromRoomQuery(RoomQuery query) => new JoinedPlayerInfo(query.PlayerName, query.RoomName);
    }
}
