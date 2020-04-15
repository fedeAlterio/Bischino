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
using Bischino.Bischino;
using Bischino.Controllers.Extensions;
using Bischino.Controllers.Queries;
using Bischino.Model;
using Bischino.Test;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Bischino.Controllers
{
    public class RoomsController : ModelController<Room>
    {
        private static readonly RoomsCollection RoomsCollection = new RoomsCollection();
        
        public RoomsController(IOwnedModelCollectionService<Room> collectionService) : base(collectionService)
        {
            var index = Builders<Room>.IndexKeys.Ascending(room => room.CreationDate);
            var options = new CreateIndexOptions { ExpireAfter = TimeSpan.FromHours(1) };
            var createIndexModel = new CreateIndexModel<Room>(index, options);
            MongoService.MongoCollection.Indexes.CreateOne(createIndexModel);
        }




        private void ValidateCreate(Room room)
        {
            if (room.MinPlayers > room.MaxPlayers)
                ThrowValidationEx("Max should be a value greater or equal to Min");
           
            if (room.Name.Any(char.IsWhiteSpace))
               ThrowValidationEx("Please insert a valid room name");
            
        }

        [ValidateModel]
        public IActionResult Create([FromBody] Room room)
        {
            try
            {
                ValidateCreate(room);

                room.PendingPlayers = new List<string>();
                room.IsMatchStarted = false;
                room.IsFull = false;
                room.CreationDate = DateTime.Now;
                var rm = new RoomManager(room);
                rm.WaitingRoomDisconnectedPlayer += RoomManager_WaitingRoomDisconnectedPlayer;
                rm.RoomClosed += RoomManager_RoomClosed;

                RoomsCollection.Add(rm);
                return Ok();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch
            {
                return BadRequest();
            }
        }



        public IActionResult GetRooms([FromBody] RoomSearchQuery query)
        {
            try
            {
                var rooms = RoomsCollection.GetRooms(query);
                return Ok(new ValuedResponse(rooms));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }




        public IActionResult GetJoinedPLayers([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(roomQuery.RoomName);
                var joinedPlayers = roomManager.Room.PendingPlayers;

                lock (roomManager)
                {
                    roomManager.NotifyToBePinged(roomQuery.PlayerName);
                }
                return Ok(new ValuedResponse(joinedPlayers));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }




        public IActionResult IsMatchStarted([FromBody] string roomName)
        {
            try
            {
                var room = RoomsCollection.Get(roomName);
                return Ok(new ValuedResponse(room.IsGameStarted));
            }
            catch (Exception)
            {
                return Ok(new ValuedResponse(false));
            }
        }



        private void ValidateStart(Room room, RoomManager roomManager)
        {
            if (roomManager.IsGameStarted)
                ThrowValidationEx("This game is already started");
            
            if (room.PendingPlayers.Count < room.MinPlayers)
                ThrowValidationEx("There are not enough players to start the game");
        }

        public IActionResult Start([FromBody] string roomName)
        {
            try
            {
                var roomManager = RoomsCollection.Get(roomName);
                var room = roomManager.Room;
                lock (roomManager)
                {
                    ValidateStart(room, roomManager);

                    roomManager.Start(room.Name, room.PendingPlayers);
                    var first = RoomsCollection.First();
                    if (first.StartTime <= DateTime.Now.Subtract(TimeSpan.FromDays(1)))
                        RoomsCollection.Remove(first);
                    
                    foreach (var player in room.PendingPlayers)
                        roomManager.NewSubscriber(player); 
                    roomManager.NotifyAll();
                }

                return Ok();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }




        private void ValidateJoin(Room room, RoomQuery roomQuery)
        {
            if (room.PendingPlayers.Count == room.MaxPlayers)
                ThrowValidationEx("The room is full");

            if (room.PendingPlayers.Contains(roomQuery.PlayerName))
                ThrowValidationEx("A player with the same username has already joined the room");
        }

        public IActionResult Join([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(roomQuery.RoomName);
                lock (roomManager)
                {
                    var room = roomManager.Room;
                    ValidateJoin(room, roomQuery);

                    roomManager.NotifyToBePinged(roomQuery.PlayerName);

                    room.PendingPlayers.Add(roomQuery.PlayerName);
                    room.IsFull = room.PendingPlayers.Count == room.MaxPlayers;
                }

                roomManager.NotifyToBePinged(roomQuery.PlayerName);
                return Ok();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch
            {
                return BadRequest();
            }
        }



        private void UnJoinPlayer(RoomQuery roomQuery)
        {
            var roomManager = RoomsCollection.Get(roomQuery.RoomName);
            lock (roomManager)
            {
                var room = roomManager.Room;

                room.PendingPlayers.Remove(roomQuery.PlayerName);
                room.IsFull = false;

                if (!room.PendingPlayers.Any())
                    RoomsCollection.Remove(roomManager);
            }
        }

        public IActionResult UnJoin([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(roomQuery.RoomName);
                roomManager.NotifyDisconnected(roomQuery.PlayerName);
                return Ok();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }




        public IActionResult MakeABet([FromBody] RoomQuery<int> betQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(betQuery.RoomName);
                var gameManager = roomManager.GameManager;
                lock (roomManager)
                {
                    ValidateTurn(gameManager, betQuery.PlayerName);
                    var bet = betQuery.Data;
                    gameManager.CurrentPlayer.NewBet(bet);
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch
            {
                return BadRequest();
            }
        }




        public IActionResult DropCard([FromBody] RoomQuery<string> dropQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(dropQuery.RoomName);
                var gameManager = roomManager.GameManager;
                lock (roomManager)
                {
                    ValidateTurn(gameManager, dropQuery.PlayerName);
                    var cardName = dropQuery.Data;
                    gameManager.CurrentPlayer.DropCard(cardName);
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch(Exception e)
            {
                return BadRequest(new ValuedResponse { Message = e.Message });
            }
        }




        public IActionResult DropPaolo([FromBody] RoomQuery<bool> dropPaolo)
        {
            try
            {
                var roomManager = RoomsCollection.Get(dropPaolo.RoomName);
                var gameManager = roomManager.GameManager;
                lock (roomManager)
                {
                    ValidateTurn(gameManager, dropPaolo.PlayerName);
                    var isMax = dropPaolo.Data;
                    gameManager.CurrentPlayer.DropPaolo(isMax);
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch
            {
                return BadRequest();
            }
        }




        public IActionResult NextPhase([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(roomQuery.RoomName);
                var gameManager = roomManager.GameManager;
                lock (roomManager)
                {
                    ValidateTurn(gameManager, roomQuery.PlayerName);

                    gameManager.NewPhase();
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch
            {
                return BadRequest();
            }
        }




        public IActionResult NextTurn([FromBody] RoomQuery<string> dropQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(dropQuery.RoomName);
                var gameManager = roomManager.GameManager;
                lock (roomManager)
                {
                    ValidateTurn(gameManager, dropQuery.PlayerName);

                    gameManager.NewTurn();
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch
            {
                return BadRequest();
            }
        }




        public async Task<IActionResult> GetMatchSnapshot([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(roomQuery.RoomName);
                var snapshot = await roomManager.PopFirstSnapshotAsync(roomQuery.PlayerName);

                return Ok(new ValuedResponse(snapshot));
            }
            catch (Exception e) 
            {
                return BadRequest();
            }
        }

        public IActionResult GetMatchSnapshotForced([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(roomQuery.RoomName);
                var snapshot = roomManager.GameManager.GetSnapshot(roomQuery.PlayerName);

                return Ok(new ValuedResponse(snapshot));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }



        public IActionResult GetGameInfo([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomManager = RoomsCollection.Get(roomQuery.RoomName);
                return Ok(new ValuedResponse(roomManager));
            }
            catch (ValidationException e)
            {
                return BadRequest(new ValuedResponse {Message = e.Message});
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }


        private void RoomManager_RoomClosed(object sender, EventArgs e)
        {
            var roomManager = sender as RoomManager;
            RoomsCollection.Remove(roomManager);
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


        private static void ThrowValidationEx(string message) => throw new ValidationException(message);
    }
}
