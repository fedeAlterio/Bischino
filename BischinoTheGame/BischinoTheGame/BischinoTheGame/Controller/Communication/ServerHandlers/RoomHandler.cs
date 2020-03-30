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
        public static RoomHandler Instance => _roomHandler ??= new RoomHandler();
        protected override string BaseUrl { get; } = "rooms/";

        public RoomHandler()
        {

        }

        public async Task Create(Room room)
        {
            await Post(room);
        }

        public async Task<IList<Room>> GetRooms(RoomSearchQuery query)
        {
            var ret = await Get<IList<Room>>(query);
            return ret;
        }

        public async Task Join(RoomQuery roomQuery)
        {
            await Post(roomQuery);
        }

        public Task<IList<string>> GetJoinedPLayers(RoomQuery roomQuery)
        {
            var ret = Get<IList<string>>(roomQuery);
            return ret;
        }

        public Task<bool> IsMatchStarted(string roomName)
        {
            var ret = Get<bool>(roomName);
            return ret;
        }

        public async Task Start(string roomName)
        {
            await Post(roomName);
        }

        public Task<MatchSnapshot> GetMatchSnapshot(RoomQuery roomQuery)
        {
            var ret = Get<MatchSnapshot>(roomQuery);
            return ret;
        }

        public async Task MakeABet(RoomQuery roomQuery)
        {
            await Post(roomQuery);
        }

        public async Task DropCard(RoomQuery<string> roomQuery)
        {
            await Post(roomQuery);
        }

        public async Task NextTurn(RoomQuery roomQuery)
        {
            await Post(roomQuery);
        }

        public async Task NextPhase(RoomQuery roomQuery)
        {
            await Post(roomQuery);
        }

        public void SubscribeMatchSnapshotUpdates(RoomQuery roomQuery)
        {
            //await WebSocket.ConnectAsync(WebSocketServerUri, CancellationToken.None);
            //await WebSocket.SendAsync(roomQuery);
            GetSnapshotRoutine(roomQuery);
        }

        private async void GetSnapshotRoutine(RoomQuery roomQuery)
        {
            while (true)
            {
                try
                {
                   var snapshot = await GetMatchSnapshot(roomQuery);
                    MatchSnapshotUpdated?.Invoke(this, snapshot);
                }
                catch (Exception e)
                {
                }
            }

            throw new Exception("Socket closed");
        }

        public async Task DropPaolo(RoomQuery<bool> paoloQuery)
        {
            await Post(paoloQuery);
        }
    }
}
