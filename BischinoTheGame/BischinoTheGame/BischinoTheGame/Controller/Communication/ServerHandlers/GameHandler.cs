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
        public event EventHandler<MatchSnapshot> MatchSnapshotUpdated;

        private static GameHandler _roomHandler;
        public static GameHandler Instance => _roomHandler ??= new GameHandler();
        protected override string BaseUrl { get; } = "rooms/";
        private bool _matchSnapshotLost;

        public Task Create(Room room)
            => Post(room);

        public Task<IList<Room>> GetRooms(RoomSearchQuery query)
            => Get<IList<Room>>(query);

        public Task Join(RoomQuery roomQuery)
            => Post(roomQuery);

        public Task<IList<string>> GetJoinedPLayers(RoomQuery roomQuery)
            => Get<IList<string>>(roomQuery);

        public Task<bool> IsMatchStarted(string roomName)
            => Get<bool>(roomName);

        public Task Start(string roomName)
            => Post(roomName);

        public async Task<MatchSnapshot> GetMatchSnapshot(RoomQuery roomQuery, CancellationToken token)
        {
            try
            {
                var ret = _matchSnapshotLost ?
                     await GetMatchSnapshotForced(roomQuery, token) :
                     await Get<MatchSnapshot>(roomQuery, token);
                _matchSnapshotLost = false;
                return ret;
            }
            catch (TimeoutException)
            {
                return await GetMatchSnapshotForced(roomQuery, token);
            }
            catch
            {
                _matchSnapshotLost = true;
                throw;
            }
        }


        private Task<MatchSnapshot> GetMatchSnapshotForced(RoomQuery roomQuery, CancellationToken token)
            => Get<MatchSnapshot>(roomQuery, token);

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
