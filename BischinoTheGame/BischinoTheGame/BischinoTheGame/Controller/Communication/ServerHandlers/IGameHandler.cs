using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;

namespace BischinoTheGame.Controller.Communication.ServerHandlers
{
    public interface IGameHandler
    {
        Task<Room> Create(Room room);
        Task<IList<Room>> GetRooms(RoomSearchQuery query);
        Task Join(RoomQuery roomQuery);
        Task<Room> JoinPrivate(RoomQuery roomQuery);
        Task<WaitingRoomInfo> GetWaitingRoomInfo(RoomQuery roomQuery);
        Task<bool> IsMatchStarted(string roomName);
        Task Start(string roomName);
        Task<MatchSnapshot> GetMatchSnapshot(RoomQuery roomQuery, CancellationToken token);
        Task MakeABet(RoomQuery roomQuery);
        Task DropCard(RoomQuery<string> roomQuery);
        Task NextTurn(RoomQuery roomQuery);
        Task NextPhase(RoomQuery roomQuery);
        Task DropPaolo(RoomQuery<bool> paoloQuery);
        Task UnJoin(RoomQuery roomQuery);
        Task<RoomManager> GetGameInfo(RoomQuery roomQuery);
        Task<int> GetCurrentSnapshotNumber(RoomQuery roomQuery, CancellationToken token);
        Task<MatchSnapshot> GetMatchSnapshotForced(RoomQuery roomQuery, CancellationToken token);
        Task AddBot(RoomQuery roomQuery);
        Task RemoveABot(RoomQuery roomQuery);
    }
}
