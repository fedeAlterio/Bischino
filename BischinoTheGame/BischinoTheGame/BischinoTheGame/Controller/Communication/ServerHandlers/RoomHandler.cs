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
    public class RoomHandler : ServerHandler, IRoomHandler
    {
        public event EventHandler<MatchSnapshot> MatchSnapshotUpdated;

        private static RoomHandler _roomHandler;
        private CancellationTokenSource _updateTokenSource;
        public static RoomHandler Instance => _roomHandler ??= new RoomHandler();
        protected override string BaseUrl { get; } = "rooms/";


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

        public Task<MatchSnapshot> GetMatchSnapshot(RoomQuery roomQuery, CancellationToken token)
            => Get<MatchSnapshot>(roomQuery);

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

        public void SubscribeMatchSnapshotUpdates(RoomQuery roomQuery)
        {
            _updateTokenSource = new CancellationTokenSource();
            GetSnapshotRoutine(roomQuery, _updateTokenSource.Token);
        }

        public void UnsubscribeMatchSnapshotUpdates()
            => _updateTokenSource?.Cancel();

        public Task<RoomManager> GetGameInfo(RoomQuery roomQuery)
            => Get<RoomManager>(roomQuery);

        private async void GetSnapshotRoutine(RoomQuery roomQuery, CancellationToken token)
        {
            while (!_updateTokenSource.IsCancellationRequested)
            {
                try
                {
                    var snapshot = await GetMatchSnapshot(roomQuery, token);
                    MatchSnapshotUpdated?.Invoke(this, snapshot);
                }
                catch (TaskCanceledException) when (token.IsCancellationRequested)
                {
                }
                catch { }
            }
            }


    }
}
