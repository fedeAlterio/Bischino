using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Bischino.Bischino;
using Bischino.Controllers.Extensions;
using Bischino.Model;

namespace Bischino.Controllers
{
    public class RoomManager
    {
        public readonly object Lock = new object();
        public string RoomName { get; }
        public GameManager GameManager { get; private set; }
        public bool IsGameStarted { get; private set; }
        public DateTime? StartTime { get; private set; } = DateTime.Now;

        private readonly Dictionary<string, MatchSnapshotWrapper> _snapshotDictionary = new Dictionary<string, MatchSnapshotWrapper>();


        public RoomManager(string roomName)
        {
            RoomName = roomName;
        }


        public void NewSubscriber(string playerName)
        {
            var snapshotWrapper = new MatchSnapshotWrapper();
            _snapshotDictionary.TryAdd(playerName, snapshotWrapper);
        }

        public void NotifyAll()
        {
            foreach (var (playerName, snapshotWrapper) in _snapshotDictionary)
            {
                var snapshot = GameManager.GetSnapshot(playerName);
                snapshotWrapper.NotifyNew(snapshot);
            }
        }

        public Task<MatchSnapshot> PopFirstSnapshotAsync(string playerName) => _snapshotDictionary[playerName].PopFirstAsync();

        public void Start(string roomName, IList<string> roomPendingPlayers)
        {
            if(IsGameStarted)
                throw new Exception("Match already started");

            IsGameStarted = true;
            StartTime = DateTime.Now;
            GameManager = new GameManager(roomName, roomPendingPlayers);
            GameManager.StartGame();
        }
    }
}
