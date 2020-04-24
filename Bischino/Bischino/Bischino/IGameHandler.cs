using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Controllers;
using Bischino.Controllers.Queries;
using Bischino.Controllers.Responses;
using Bischino.Model;

namespace Bischino.Bischino
{
    public interface IGameHandler
    {
        Room Create(Room room);
        IList<Room> GetRooms(RoomSearchQuery query);
        WaitingRoomInfo GetWaitingRoomInfo(RoomQuery roomQuery);
        bool IsMatchStarted(string roomName);
        void Start(string roomName);
        void Join(RoomQuery roomQuery);
        Room JoinPrivate(RoomQuery roomQuery);
        void UnJoin(RoomQuery roomQuery);
        void AddBot(RoomQuery roomQuery);
        void RemoveABot(RoomQuery roomQuery);
        void MakeABet(RoomQuery<int> betQuery);
        void DropCard(RoomQuery<string> dropQuery);
        void DropPaolo(RoomQuery<bool> dropPaolo);
        void NextPhase(RoomQuery roomQuery);
        void NextTurn(RoomQuery<string> dropQuery);
        Task<MatchSnapshot> GetMatchSnapshot(RoomQuery roomQuery);
        MatchSnapshot GetMatchSnapshotForced(RoomQuery roomQuery);
        int GetCurrentSnapshotNumber(RoomQuery roomQuery);
        RoomManager GetGameInfo(RoomQuery roomQuery);
    }
}

