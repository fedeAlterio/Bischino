using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;
using Newtonsoft.Json;

namespace BischinoTheGame.Controller.Communication.ServerHandlers
{
    public class GameHandler : ServerHandler, IGameHandler
    {
        private static GameHandler _roomHandler;
        public static GameHandler Instance => _roomHandler ??= new GameHandler();
        protected override string BaseUri { get; } = "rooms/";
        private bool _matchSnapshotLost;
        private MatchSnapshot LastSnapshot;

        public Task<Room> Create(Room room)
            => Get<Room>(room);

        public Task<IList<Room>> GetRooms(RoomSearchQuery query)
            => Get<IList<Room>>(query);

        public Task Join(RoomQuery roomQuery)
            => Post(roomQuery);

        public Task<Room> JoinPrivate(RoomQuery roomQuery)
            => Get<Room>(roomQuery);

        public Task<WaitingRoomInfo> GetWaitingRoomInfo(RoomQuery roomQuery)
            => Get<WaitingRoomInfo>(roomQuery);

        public Task<bool> IsMatchStarted(string roomName)
            => Get<bool>(roomName);

        public Task Start(string roomName)
            => Post(roomName);

        public Task<int> GetCurrentSnapshotNumber(RoomQuery roomQuery, CancellationToken token)
            => Get<int>(roomQuery, token);

        public Task<MatchSnapshot> GetMatchSnapshot(RoomQuery roomQuery, CancellationToken token)
            => Get<MatchSnapshot>(roomQuery, token);

        public Task<MatchSnapshot> GetMatchSnapshotForced(RoomQuery roomQuery, CancellationToken token)
            => Get<MatchSnapshot>(roomQuery, token);

        public Task AddBot(RoomQuery roomQuery)
            => Post(roomQuery);

        public Task RemoveABot(RoomQuery roomQuery)
            => Post(roomQuery);

        public Task MakeABet(RoomQuery roomQuery)
            => Post(roomQuery);

        public Task DropCard(RoomQuery<string> roomQuery)
            => Post(roomQuery);

        public Task NextTurn(RoomQuery roomQuery)
            => Post(roomQuery);

        public Task NextPhase(RoomQuery roomQuery)
            => Post(roomQuery);

        public Task DropPaolo(RoomQuery<bool> paoloQuery)
            => Post(paoloQuery);

        public Task UnJoin(RoomQuery roomQuery)
            => Post(roomQuery);

        public Task<RoomManager> GetGameInfo(RoomQuery roomQuery)
            => Get<RoomManager>(roomQuery);
    }
}
