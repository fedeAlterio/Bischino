using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skribble.Model;

namespace Bischino.Skribble
{
    public class RoomManager
    {
        private readonly ConcurrentDictionary<string, AsyncQUeue<IEnumerable<Line>>> _playersDrawDictionary = new ConcurrentDictionary<string, AsyncQUeue<IEnumerable<Line>>>();
        public Room Room { get; private set; } = new Room {Name = "Room"};
        public IList<Player> Players { get; } = new List<Player>();
        


        public async Task<IEnumerable<Line>> GetDrawUpdate(string username)
        {
            var queue = _playersDrawDictionary[username];
            var totUpdates = queue.ActualCount();

            if (totUpdates == 0)
                return await queue.PopAsync();

            var lines = new List<Line>();
            for (int i = 0; i < totUpdates; i++)
                lines.AddRange(await queue.PopAsync());

            return lines;
        }



        public void AddDrawUpdate(string username, IEnumerable<Line> lines)
        {
            var otherPlayers = from player in Players where player.Username != username select player;
            foreach(var player in otherPlayers)
                _playersDrawDictionary[player.Username].Add(lines);
        }



        public void NotifyPlayerJoined(string username)
        {
            Players.Add(new Player(username));
            _playersDrawDictionary.TryAdd(username, new AsyncQUeue<IEnumerable<Line>>());
        }
    }
}
