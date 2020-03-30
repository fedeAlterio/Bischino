using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;

namespace BischinoTheGame.Controller.Communication.ServerHandlers
{
    public interface IRoomHandler
    {
        event EventHandler<MatchSnapshot> MatchSnapshotUpdated;
        Task Create(Room room);
        Task<IList<Room>> GetRooms(RoomSearchQuery query);
        Task Join(RoomQuery roomQuery);
        Task<IList<string>> GetJoinedPLayers(RoomQuery roomQuery);
        Task<bool> IsMatchStarted(string roomName);
        Task Start(string roomName);
        Task<MatchSnapshot> GetMatchSnapshot(RoomQuery roomQuery);
        Task MakeABet(RoomQuery roomQuery);
        Task DropCard(RoomQuery<string> roomQuery);
        Task NextTurn(RoomQuery roomQuery);
        Task NextPhase(RoomQuery roomQuery);
        Task DropPaolo(RoomQuery<bool> paoloQuery);
        void SubscribeMatchSnapshotUpdates(RoomQuery roomQuery);   
    }
}
