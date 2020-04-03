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
using Timer = System.Timers.Timer;

namespace Bischino.Controllers
{
    public class RoomManager
    {
        public event EventHandler<RoomQuery> WaitingRoomDisconnectedPlayer;
        public event EventHandler RoomClosed;

        private const int InGameTimeout = 15 * 1000; //ms
        private const int WaitingRoomTimeout = 7 * 1000; //ms
        private const int WinPhaseTimeout = 10 * 1000;

        public string RoomName => Room.Name;
        public GameManager GameManager { get; private set; }
        public bool IsGameStarted { get; private set; }
        public DateTime? StartTime { get; private set; } = DateTime.Now;
        public Room Room { get; }

        private ConcurrentDictionary<string, Timer> _pendingPlayersTimerDictionary = new ConcurrentDictionary<string, Timer>();
        private TimeoutTimer<Player> _inGameTimer;
        private Player _currentPlayer;
        private readonly ConcurrentDictionary<string, MatchSnapshotWrapper> _snapshotDictionary = new ConcurrentDictionary<string, MatchSnapshotWrapper>();

        public RoomManager(Room room)
        {
            Room = room;
        }



        public void NotifyToBePinged(string playerName, bool resetPing = true)
        {
            if(_pendingPlayersTimerDictionary.TryGetValue(playerName, out var timer))
            {
                if (resetPing)
                {
                    timer.Stop();
                    timer.Start();
                }
            }
            else
            {
                timer = new Timer(WaitingRoomTimeout);
                timer.Elapsed += (sender, _) =>
                {
                    (sender as Timer).Stop();
                    Timer_TimeoutEvent(playerName);
                };
                if(_pendingPlayersTimerDictionary.TryAdd(playerName, timer))
                    timer.Start();
            }
        }

        private void Timer_TimeoutEvent(string playerName)
        {
            WaitingRoomDisconnectedPlayer?.Invoke(this, new RoomQuery { PlayerName = playerName, RoomName = RoomName });
        }

        public void StopWaitingRoomTimers()
        {
            foreach (var (_, timer) in _pendingPlayersTimerDictionary)
                timer.Stop();
            _pendingPlayersTimerDictionary = null;
        }


        public void NotifyDisconnected(string playerName)
        {
            _pendingPlayersTimerDictionary[playerName].Stop();
            Timer_TimeoutEvent(playerName);
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

            StopWaitingRoomTimers();
            IsGameStarted = true;
            StartTime = DateTime.Now;
            GameManager = new GameManager(roomName, roomPendingPlayers);
            GameManager.CurrentPlayerChanged += GameManager_CurrentPlayerChangedEvent;
            GameManager.EndOfMatch += GameManager_EndOfMatch;
            GameManager.StartGame();
        }

        private async void GameManager_EndOfMatch(object sender, IList<Player> e)
        {
            _inGameTimer?.Stop();
            await Task.Delay(WinPhaseTimeout);
            RoomClosed?.Invoke(this, EventArgs.Empty);
        }

        private void GameManager_CurrentPlayerChangedEvent(object sender, Player player)
        {
            if (_currentPlayer == player)
                return;
            _currentPlayer = player;
            _inGameTimer?.Stop();
            _inGameTimer = new TimeoutTimer<Player>(InGameTimeout, player);
            _inGameTimer.TimeoutEvent += _inGameTimer_TimeoutEvent;
            _inGameTimer.Start();
        }

        private void _inGameTimer_TimeoutEvent(object sender, Player player)
        {
            //NotifyAll(player.Name);
            _inGameTimer?.Stop();
            player.IsIdled = true;
            GameManager.NotifyIdled(player.Name);
            NotifyAll();
            //RoomClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
