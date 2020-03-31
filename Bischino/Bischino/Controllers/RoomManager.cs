using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Bischino.Bischino;
using Bischino.Controllers.Extensions;
using Bischino.Helpers;
using Bischino.Model;

namespace Bischino.Controllers
{
    public class RoomManager
    {
        public event EventHandler<RoomQuery> WaitingRoomDisconnectedPlayer;
        private const int InGameTimeout = 120 * 1000; //ms
        private const int WaitingRoomTimeout = 7 * 1000; //ms
        public readonly object Lock = new object();
        public string RoomName { get; }
        public GameManager GameManager { get; private set; }
        public bool IsGameStarted { get; private set; }
        public DateTime? StartTime { get; private set; } = DateTime.Now;

        private ConcurrentDictionary<string, TimeoutTimer<string>> _pendingPlayersTimerDictionary = new ConcurrentDictionary<string, TimeoutTimer<string>>();
        private TimeoutTimer<Player> InGameTimer;

        private Player _currentPlayer;

        private readonly Dictionary<string, MatchSnapshotWrapper> _snapshotDictionary = new Dictionary<string, MatchSnapshotWrapper>();


        public RoomManager(string roomName)
        {
            RoomName = roomName;
        }



        public void NotifyToBePinged(string playerName, bool resetPing = true)
        {
            if(_pendingPlayersTimerDictionary.TryGetValue(playerName, out var timer))
            {
                if (resetPing)
                    timer.Reset();
            }
            else
            {
                timer = new TimeoutTimer<string>(WaitingRoomTimeout, playerName);
                timer.TimeoutEvent += (_, player) => WaitingRoomDisconnectedPlayer?.Invoke(this, new RoomQuery{PlayerName = playerName, RoomName = RoomName});
                if(_pendingPlayersTimerDictionary.TryAdd(playerName, timer))
                    timer.Start();
            }
        }



        public void StopWaitingRoomTimers()
        {
            foreach (var (_, timer) in _pendingPlayersTimerDictionary)
                timer.Stop();
            _pendingPlayersTimerDictionary = null;
        }

        public void NewSubscriber(string playerName)
        {
            var snapshotWrapper = new MatchSnapshotWrapper();
            _snapshotDictionary.TryAdd(playerName, snapshotWrapper);
        }

        public void NotifyAll(string disconnectedPlayer = null)
        {
            foreach (var (playerName, snapshotWrapper) in _snapshotDictionary)
            {
                var snapshot = GameManager.GetSnapshot(playerName);
                snapshot.DisconnectedPlayer = disconnectedPlayer;
                snapshotWrapper.NotifyNew(snapshot);
            }
        }

        public Task<MatchSnapshot> PopFirstSnapshotAsync(string playerName) => _snapshotDictionary[playerName].PopFirstAsync();

        public void Start(string roomName, IList<string> roomPendingPlayers)
        {
            if(IsGameStarted)
                throw new Exception("Match already started");

            StopWaitingRoomTimers();
            IsGameStarted = true;
            StartTime = DateTime.Now;
            GameManager = new GameManager(roomName, roomPendingPlayers);
            GameManager.CurrentPlayerChanged += GameManager_CurrentPlayerChangedEvent;
            GameManager.EndOfMatch += GameManager_EndOfMatch;
            GameManager.StartGame();
        }

        private void GameManager_EndOfMatch(object sender, IList<Player> e)
        {
            InGameTimer?.Stop();
        }

        private void GameManager_CurrentPlayerChangedEvent(object sender, Player player)
        {
            if (_currentPlayer == player)
                return;
            _currentPlayer = player;
            InGameTimer?.Stop();
            InGameTimer = new TimeoutTimer<Player>(InGameTimeout, player);
            InGameTimer.TimeoutEvent += (_, p) => NotifyAll(p.Name);
            InGameTimer.Start();
        }
    }
}
