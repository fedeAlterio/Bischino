using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Model;
using Bischino.Model.Exeptions;
using Bischino.Model.Responses;

namespace Bischino.Model
{
    public class GameHandler : IGameHandler
    {
        private readonly RoomsCollection _roomsCollection = new RoomsCollection();


        private void ValidateCreate(Room room)
        {
            if (room.MinPlayers > room.MaxPlayers)
                ThrowValidationEx("Max should be a value greater or equal to Min");

            if (room.Name.Any(char.IsWhiteSpace))
                ThrowValidationEx("Please insert a valid room name");

        }
        public Room Create(Room room)
        {
            ValidateCreate(room);

            room.PendingPlayers = new List<string>();
            room.IsMatchStarted = false;
            room.IsFull = false;
            room.CreationDate = DateTime.Now;
            room.IsPrivate ??= false;
            var rm = new RoomManager(room);
            rm.WaitingRoomDisconnectedPlayer += RoomManager_WaitingRoomDisconnectedPlayer;
            rm.MatchEnded += RoomManager_RoomClosed;

            _roomsCollection.Add(rm);

            return room;
        }




        public IList<Room> GetRooms(RoomSearchQuery query)
        {
            var rooms = _roomsCollection.GetRooms(query);
            return rooms;
        }




        public WaitingRoomInfo GetWaitingRoomInfo(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);

            WaitingRoomInfo waitingRoomInfo;
            lock (roomManager)
            {
                roomManager.NotifyToBePinged(roomQuery.PlayerName);
                waitingRoomInfo = new WaitingRoomInfo { BotCounter = roomManager.Room.BotCounter ?? 0, NotBotPlayers = roomManager.Room.NotBotPlayers.ToList() };
            }

            return waitingRoomInfo;
        }




        public bool IsMatchStarted(string roomName)
        {
            var room = _roomsCollection.Get(roomName);
            return room.IsGameStarted;
        }




        private void ValidateStart(Room room, RoomManager roomManager)
        {
            if (roomManager.IsGameStarted)
                ThrowValidationEx("This game is already started");


            if (room.PendingPlayers.Count + roomManager.Room.BotCounter < room.MinPlayers)
                ThrowValidationEx("There are not enough players to start the game");
        }

        public void Start(string roomName)
        {
            var roomManager = _roomsCollection.Get(roomName);
            var room = roomManager.Room;
            lock (roomManager)
            {
                ValidateStart(room, roomManager);

                roomManager.Start();
                roomManager.GameManager.MatchUpdated += (_, __) => roomManager.NotifyAll();
                var first = _roomsCollection.First();
                if (first.StartTime <= DateTime.Now.Subtract(TimeSpan.FromDays(1)))
                    _roomsCollection.Remove(first);

                foreach (var player in room.NotBotPlayers)
                    roomManager.NewSubscriber(player);
                roomManager.NotifyAll();
            }
        }




        private void ValidateJoin(RoomManager roomManager, RoomQuery roomQuery)
        {
            if (roomManager.IsRoomFull())
                ThrowValidationEx("The room is full");

            if (roomManager.Room.PendingPlayers.Contains(roomQuery.PlayerName))
                ThrowValidationEx("A player with the same username has already joined the room");

            if (roomManager.IsGameStarted)
                ThrowValidationEx("The game is already started");
        }

        private void Join(RoomManager roomManager, RoomQuery roomQuery)
        {
            lock (roomManager)
            {
                var room = roomManager.Room;
                ValidateJoin(roomManager, roomQuery);

                roomManager.NotifyToBePinged(roomQuery.PlayerName);

                room.AddPlayingPlayer(roomQuery.PlayerName);
                room.IsFull = roomManager.IsRoomFull();
            }
            roomManager.NotifyToBePinged(roomQuery.PlayerName);
        }

        public void Join(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);
            Join(roomManager, roomQuery);
        }




        private void ValidateJoinPrivate(RoomQuery roomQuery)
        {
            if (roomQuery.RoomNumber is null)
                ThrowValidationEx("The room number is required");
        }

        public Room JoinPrivate(RoomQuery roomQuery)
        {
            ValidateJoinPrivate(roomQuery);

            var roomManager = _roomsCollection.Get(roomQuery.RoomNumber.Value);
            Join(roomManager, roomQuery);
            return roomManager.Room;
        }




        private void UnJoinPlayer(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);
            lock (roomManager)
            {
                var room = roomManager.Room;

                room.RemovePlayingPlayer(roomQuery.PlayerName);
                room.IsFull = false;

                if (!room.NotBotPlayers.Any())
                    _roomsCollection.Remove(roomManager);
            }
        }

        public void UnJoin(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);
            roomManager.NotifyDisconnected(roomQuery.PlayerName);
        }




        private void ValidateAddBot(RoomManager roomManager, string playerName)
        {
            if (roomManager.IsRoomFull())
                throw new GameException("The room is full");

            if (roomManager.IsGameStarted)
                ThrowValidationEx("The game is already started");

            if(roomManager.Room.GetActualHost() != playerName)
                ThrowValidationEx("Only the host can add a bot");
        }

        public void AddBot(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);
            ValidateAddBot(roomManager, roomQuery.PlayerName);

            roomManager.AddBot();
        }




        public void ValidateRemoveABot(RoomManager roomManager, string playerName)
        {
            if (roomManager.Room.BotCounter == 0)
                throw new GameException("There are no bot in this room");

            if (roomManager.IsGameStarted)
                ThrowValidationEx("The game is already started");

            if (roomManager.Room.GetActualHost() != playerName)
                ThrowValidationEx("Only the host can remove a bot");
        }

        public void RemoveABot(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);
            ValidateRemoveABot(roomManager, roomQuery.PlayerName);

            roomManager.RemoveABot();
        }




        public void MakeABet(RoomQuery<int> betQuery)
        {
            var roomManager = _roomsCollection.Get(betQuery.RoomName);
            var gameManager = roomManager.GameManager;
            lock (roomManager)
            {
                ValidateTurn(gameManager, betQuery.PlayerName);
                var bet = betQuery.Data;
                gameManager.CurrentPlayer.NewBet(bet);
            }
        }




        public void DropCard(RoomQuery<string> dropQuery)
        {
            var roomManager = _roomsCollection.Get(dropQuery.RoomName);
            var gameManager = roomManager.GameManager;
            lock (roomManager)
            {
                ValidateTurn(gameManager, dropQuery.PlayerName);
                var cardName = dropQuery.Data;
                gameManager.CurrentPlayer.DropCard(cardName);
            }

        }




        public void DropPaolo(RoomQuery<bool> dropPaolo)
        {
            var roomManager = _roomsCollection.Get(dropPaolo.RoomName);
            var gameManager = roomManager.GameManager;
            lock (roomManager)
            {
                ValidateTurn(gameManager, dropPaolo.PlayerName);
                var isMax = dropPaolo.Data;
                gameManager.CurrentPlayer.DropPaolo(isMax);
            }
        }




        public async Task<MatchSnapshot> GetMatchSnapshot(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);
            var snapshot = await roomManager.PopFirstSnapshotAsync(roomQuery.PlayerName);
            return snapshot;
        }




        public MatchSnapshot GetMatchSnapshotForced(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);
            var snapshot = roomManager.GameManager.GetSnapshot(roomQuery.PlayerName);
            return snapshot;
        }




        public int GetCurrentSnapshotNumber(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);
            var version = roomManager.GameManager.Version;
            return version;
        }




        public RoomManager GetGameInfo(RoomQuery roomQuery)
        {
            var roomManager = _roomsCollection.Get(roomQuery.RoomName);
            return roomManager;
        }




        private void RoomManager_RoomClosed(object sender, EventArgs e)
        {
            var roomManager = sender as RoomManager;
            _roomsCollection.Remove(roomManager);
        }



        private void RoomManager_WaitingRoomDisconnectedPlayer(object sender, RoomQuery roomQuery)
        {
            try
            {
                UnJoinPlayer(roomQuery);
            }
            catch (Exception)
            {

            }
        }




        private static void ValidateTurn(GameManager gameManager, string playerName)
        {
            if (gameManager.CurrentPlayer.Name != playerName)
                ThrowValidationEx("It's not your turn");
        }


        private static void ThrowValidationEx(string message) => throw new GameException(message);
        private const string UpdateMessage = "This application is no longer supported, please check for updates in the play store";
    }
}
