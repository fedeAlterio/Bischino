using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Bischino.Base.Controllers;
using Bischino.Base.Controllers.Filters;
using Bischino.Base.Security;
using Bischino.Base.Service;
using Bischino.Controllers;
using Bischino.Model;
using Bischino.Model.Exeptions;
using Bischino.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using MongoDB.Driver;

namespace Bischino.Model
{
    public class RoomsController : AbstractController
    {
        private readonly IGameHandler _gameHandler;

        public RoomsController(IGameHandler gameHandler)
        {
            _gameHandler = gameHandler;
        }




        [ValidateModel]
        public IActionResult Create([FromBody] Room room)
            => TryValuedOk(() => _gameHandler.Create(room));



        public IActionResult GetRooms([FromBody] RoomSearchQuery query) => TryValuedOk(() =>
        {
            var ret = _gameHandler.GetRooms(query);
            return ret;
        });



        public IActionResult GetJoinedPLayers([FromBody] RoomQuery roomQuery) => TryValuedOk(() =>
        {
            var waitingRoomInfo = _gameHandler.GetWaitingRoomInfo(roomQuery);
            var joinedPlayers = new List<string>(waitingRoomInfo.NotBotPlayers);
            for (int i = 0; i < waitingRoomInfo.BotCounter; i++)
                joinedPlayers.Add($"bot{i}");

            return joinedPlayers;
        });



        public IActionResult GetWaitingRoomInfo([FromBody] RoomQuery roomQuery)
            => TryValuedOk(() => _gameHandler.GetWaitingRoomInfo(roomQuery));


        public IActionResult IsMatchStarted([FromBody] string roomName)
            => TryValuedOk(() => _gameHandler.IsMatchStarted(roomName));


        public IActionResult Start([FromBody] string roomName)
            => TryOk(() => _gameHandler.Start(roomName));


        public IActionResult Join([FromBody] RoomQuery roomQuery)
            => TryOk(() => _gameHandler.Join(roomQuery));


        public IActionResult JoinPrivate([FromBody] RoomQuery roomQuery)
            => TryValuedOk(() => _gameHandler.JoinPrivate(roomQuery));


        public IActionResult UnJoin([FromBody] RoomQuery roomQuery)
            => TryOk(() => _gameHandler.UnJoin(roomQuery));


        public IActionResult AddBot([FromBody] RoomQuery roomQuery)
            => TryOk(() => _gameHandler.AddBot(roomQuery));


        public IActionResult RemoveABot([FromBody] RoomQuery roomQuery)
            => TryOk(() => _gameHandler.RemoveABot(roomQuery));


        public IActionResult MakeABet([FromBody] RoomQuery<int> betQuery)
            => TryOk(() => _gameHandler.MakeABet(betQuery));


        public IActionResult DropCard([FromBody] RoomQuery<string> dropQuery)
            => TryOk(() => _gameHandler.DropCard(dropQuery));


        public IActionResult DropPaolo([FromBody] RoomQuery<bool> dropPaolo)
            => TryOk(() => _gameHandler.DropPaolo(dropPaolo));


        public IActionResult NextPhase([FromBody] RoomQuery roomQuery)
            => Ok();


        public IActionResult NextTurn([FromBody] RoomQuery<string> dropQuery)
            => Ok();


        public Task<IActionResult> GetMatchSnapshot([FromBody] RoomQuery roomQuery)
            => TryValuedOkAsync( () => _gameHandler.GetMatchSnapshot(roomQuery));


        public IActionResult GetMatchSnapshotForced([FromBody] RoomQuery roomQuery)
            => TryValuedOk(() => _gameHandler.GetMatchSnapshotForced(roomQuery));


        public IActionResult GetCurrentSnapshotNumber([FromBody] RoomQuery roomQuery)
            => TryValuedOk(() => _gameHandler.GetCurrentSnapshotNumber(roomQuery));


        public IActionResult GetGameInfo([FromBody] RoomQuery roomQuery)
            => TryValuedOk(() => _gameHandler.GetGameInfo(roomQuery));
    }
}
