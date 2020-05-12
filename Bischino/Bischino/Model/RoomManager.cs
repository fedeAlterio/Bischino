using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Bischino.Model;
using Bischino.Model.Helpers;
using Timer = System.Timers.Timer;

namespace Bischino.Model
{
    public class RoomManager
    {
        public event EventHandler<RoomQuery> WaitingRoomDisconnectedPlayer;
        public event EventHandler MatchEnded;

        public int InGameTimeout { get; set; } = 50 * 1000; //ms
        private const int WaitingRoomTimeout = 17 * 1000; //ms
        private const int WinPhaseTimeout = 40 * 1000;

        public string RoomName => Room.Name;
        
        [JsonIgnore]
        public Room Room { get; }
        
        [JsonIgnore]
        public GameManager GameManager { get; private set; }
        
        public bool IsGameStarted { get; private set; }
        
        public DateTime? StartTime { get; private set; } = DateTime.Now;

        private ConcurrentDictionary<string, Timer> _pendingPlayersTimerDictionary = new ConcurrentDictionary<string, Timer>();
        private TimeoutTimer<Player> _inGameTimer;
        private Player _currentPlayer;
        private readonly ConcurrentDictionary<string, MatchSnapshotWrapper> _snapshotDictionary = new ConcurrentDictionary<string, MatchSnapshotWrapper>();

        public RoomManager(Room room)
        {
            Room = room;
        }




        public bool IsRoomFull()
        {
            var totPlayers = Room.PendingPlayers.Count;
            if(totPlayers > Room.MaxPlayers)
                throw new Exception("Too many players in this room");

            return totPlayers == Room.MaxPlayers;
        }

        public void NotifyToBePinged(string playerName, bool resetPing = true)
        {
            if (_pendingPlayersTimerDictionary is null)
                return;

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
                    OnWaitingRoomTimeout(playerName);
                };
                if(_pendingPlayersTimerDictionary.TryAdd(playerName, timer))
                    timer.Start();
            }
        }

        private void OnWaitingRoomTimeout(string playerName)
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
            OnWaitingRoomTimeout(playerName);
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
                _inGameTimer?.Stop();
                if(!GameManager.GameEnded)
                    _inGameTimer?.Start();
            }
        }



        public Task<MatchSnapshot> PopFirstSnapshotAsync(string playerName) => _snapshotDictionary[playerName].PopFirstAsync();

        public void Start()
        {
            if(IsGameStarted)
                throw new Exception("Match already started");

            StopWaitingRoomTimers();
            IsGameStarted = true;
            StartTime = DateTime.Now;
            GameManager = new GameManager(Room.Name, Room.NotBotPlayers, Room.BotCounter ?? 0);
            GameManager.CurrentPlayerChanged += OnCurrentPlayerChanged;
            GameManager.EndOfMatch += GameManager_EndOfMatch;
            GameManager.StartGame();
        }

        private async void GameManager_EndOfMatch(object sender, IList<Player> e)
        {
            _inGameTimer?.Stop();
            await Task.Delay(WinPhaseTimeout);
            MatchEnded?.Invoke(this, EventArgs.Empty);
        }

        private void OnCurrentPlayerChanged(object sender, Player player)
        {
            if (_currentPlayer == player)
                return;
            _currentPlayer = player;
            _inGameTimer?.Stop();
            _inGameTimer = new TimeoutTimer<Player>(InGameTimeout, player);
            _inGameTimer.TimeoutEvent += OnInGameTimeout;
            _inGameTimer.Start();
        }

        private void OnInGameTimeout(object sender, Player player)
        {
            _inGameTimer?.Stop();
            player.IsIdled = true;
            GameManager.NotifyIdled(player.Name);
            NotifyAll();
        }

        public void AddBot()
        {
            Room.AddBot();
        }

        public void RemoveABot()
        {
            Room.RemoveBot();
        }
    }
}
