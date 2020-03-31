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
        private static readonly BlockingSet<RoomManager> RoomManagers = new BlockingSet<RoomManager>();


        public RoomsController(IOwnedModelCollectionService<Room> collectionService) : base(collectionService)
        {
            var index = Builders<Room>.IndexKeys.Ascending(room => room.CreationDate);
            var options = new CreateIndexOptions { ExpireAfter = TimeSpan.FromHours(1) };
            var createIndexModel = new CreateIndexModel<Room>(index, options);
            MongoService.MongoCollection.Indexes.CreateOne(createIndexModel);
        }

        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] Room room)
        {
            if (room.MinPlayers > room.MaxPlayers)
                return BadRequest(new ValuedResponse {Message = "Max should be a value greater or equal to Min"});

            if (room.Name.Any(char.IsWhiteSpace))
                return BadRequest(new ValuedResponse{Message = "Please insert a valid room name"});

            if (RoomManagers.GetAll(rm => rm.RoomName.Equals(room.Name)).Count > 0)
                return BadRequest(new ValuedResponse("Another room with the same name is playing a game"));

            room.PendingPlayers = new List<string>();
            room.IsMatchStarted = false;
            room.IsFull = false;
            room.CreationDate = DateTime.Now;
            var ret = await CreateModel(room);
            var roomLock = GetRoomManager(room.Name);
            RoomManagers.Add(roomLock);
            return ret;
        }


        public async Task<IActionResult> GetRooms([FromBody] RoomSearchQuery query)
        {
            try
            {
                query.Model.IsFull = false;
                var filter = BuildFilters(query);
                var findOptions = CollectionService.FindOptionsFromQuery<Room>(query.Options);

                var cursor = await MongoService.MongoCollection.FindAsync(filter, findOptions);
                var rooms = await cursor.ToListAsync();
                return Ok(new ValuedResponse(rooms));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        private FilterDefinition<Room> BuildFilters(RoomSearchQuery query)
        {
            return CollectionService.FilterFromModel<Room>(query.Model);
        }


        public async Task<IActionResult> GetJoinedPLayers([FromBody] RoomQuery roomQuery)
        {
            try
            { 
                var room = await MongoService.MongoCollection.Aggregate()
                    .Match(room => room.Name.Equals(roomQuery.RoomName))
                    .FirstAsync(); 
                var joinedPlayers = room.PendingPlayers;

                var roomManager = GetRoomManager(roomQuery.RoomName); 
                roomManager.NotifyToBePinged(roomQuery.PlayerName);
                return Ok(new ValuedResponse(joinedPlayers));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        public IActionResult IsMatchStarted([FromBody] string roomName)
        {
            try
            {
                var room = GetRoomManager(roomName);
                return Ok(new ValuedResponse(room.IsGameStarted));
            }
            catch (Exception e)
            {
                return Ok(new ValuedResponse(false));
            }
        }


        public async Task<IActionResult> Start([FromBody] string name)
        {
            try
            {
                var cursor = await MongoService.MongoCollection.FindAsync(room => room.Name.Equals(name));
                var room = cursor.FirstOrDefault();
                var roomManager = GetRoomManager(room.Name, true);
                lock (roomManager)
                {
                   // if (room.PendingPlayers.Count < room.MinPlayers)
                     //   return BadRequest();

                    if (roomManager.IsGameStarted)
                        return Ok();


                    roomManager.Start(room.Name, room.PendingPlayers);
                    var first = RoomManagers.First();
                    if(first.StartTime <= DateTime.Now.Subtract(TimeSpan.FromDays(1)))
                        RoomManagers.Remove(first);
                }
                await MongoService.Remove(room.Id);

                foreach(var player in room.PendingPlayers)
                    roomManager.NewSubscriber(player);
                roomManager.NotifyAll();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }


        public IActionResult Join([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomName = roomQuery.RoomName;
                var roomManager = GetRoomManager(roomName, true);
                lock (roomManager.Lock)
                {
                    var room = MongoService.MongoCollection.Find(dbRoom => dbRoom.Name.Equals(roomName)).First();

                    if (room.PendingPlayers.Count == room.MaxPlayers)
                        return BadRequest();
                    if (room.PendingPlayers.Contains(roomQuery.PlayerName))
                        return BadRequest();

                    foreach (var p in room.PendingPlayers)
                        roomManager.NotifyToBePinged(p, false);

                    room.PendingPlayers.Add(roomQuery.PlayerName);
                    if(room.PendingPlayers.Count == room.MaxPlayers)
                        room.IsFull = true;
                    
                    MongoService.MongoCollection.ReplaceOne(dbRoom => dbRoom.Name.Equals(room.Name), room);
                }

                roomManager.NotifyToBePinged(roomQuery.PlayerName);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        public IActionResult UnJoin([FromBody] RoomQuery roomQuery)
        {   
            try
            {
                UnJoinPlayer(roomQuery);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        public IActionResult MakeABet([FromBody] RoomQuery<int> betQuery)
        {
            try
            {
                var roomManager = GetRoomManager(betQuery.RoomName);
                var gameManager = roomManager.GameManager;
                lock(roomManager.Lock)
                {
                    ValidateTurn(gameManager, betQuery.PlayerName);
                    var bet = betQuery.Data;
                    gameManager.CurrentPlayer.NewBet(bet);
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        public IActionResult DropCard([FromBody] RoomQuery<string> dropQuery)
        {
            try
            {
                var roomManager = GetRoomManager(dropQuery.RoomName);
                var gameManager = roomManager.GameManager;
                lock (roomManager.Lock)
                {
                    ValidateTurn(gameManager, dropQuery.PlayerName);
                    var cardName = dropQuery.Data;
                    gameManager.CurrentPlayer.DropCard(cardName);
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }


        public IActionResult DropPaolo([FromBody] RoomQuery<bool> dropPaolo)
        {
            try
            {
                var roomManager = GetRoomManager(dropPaolo.RoomName);
                var gameManager = roomManager.GameManager;
                lock (roomManager.Lock)
                {
                    ValidateTurn(gameManager, dropPaolo.PlayerName);
                    var isMax = dropPaolo.Data;
                    gameManager.CurrentPlayer.DropPaolo(isMax);
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }


        public IActionResult NextPhase([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomManager = GetRoomManager(roomQuery.RoomName);
                var gameManager = roomManager.GameManager;
                lock(roomManager.Lock)
                {
                    if (gameManager.Host.Name != roomQuery.PlayerName)
                        throw new Exception("Only the host can perform this action");

                    gameManager.NewPhase();
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        public IActionResult NextTurn([FromBody] RoomQuery<string> dropQuery)
        {
            try
            {
                var roomManager = GetRoomManager(dropQuery.RoomName);
                var gameManager = roomManager.GameManager;
                lock(roomManager.Lock)
                {
                    if (gameManager.Host.Name != dropQuery.PlayerName)
                        throw new Exception("Only the host can perform this action");

                    gameManager.NewTurn();
                }

                roomManager.NotifyAll();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> GetMatchSnapshot([FromBody] RoomQuery roomQuery)
        {
            try
            {
                var roomManager = GetRoomManager(roomQuery.RoomName, false);
                var snapshot = await roomManager.PopFirstSnapshotAsync(roomQuery.PlayerName);

                return Ok(new ValuedResponse(snapshot));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        private void ValidateTurn(GameManager gameManager, string playerName)
        {
            if (gameManager.CurrentPlayer.Name != playerName)
                throw new Exception();
        }


        private RoomManager GetRoomManager(string roomName, bool createIfDoesNotExist = true)
        {
            var roomManager = RoomManagers.GetAll(rLock => rLock.RoomName == roomName).FirstOrDefault();
            if(roomManager is null)
                if (createIfDoesNotExist)
                {
                    RoomManagers.Add(roomManager = new RoomManager(roomName));
                    roomManager.WaitingRoomDisconnectedPlayer += RoomManager_WaitingRoomDisconnectedPlayer;
                }
                else
                    throw new Exception("RoomLock does not exist");

            return roomManager;
        }

        private void UnJoinPlayer(RoomQuery roomQuery)
        {
            var roomName = roomQuery.RoomName;
            var roomLock = GetRoomManager(roomName, true);
            lock (roomLock.Lock)
            {
                var room = MongoService.MongoCollection.Find(dbRoom => dbRoom.Name.Equals(roomName)).First();
                room.PendingPlayers.Remove(roomQuery.PlayerName);
                room.IsFull = false;
               
                if (!room.PendingPlayers.Any())
                    MongoService.MongoCollection.DeleteOne(dbRoom => dbRoom.Name.Equals(room.Name));
                else
                    MongoService.MongoCollection.ReplaceOne(dbRoom => dbRoom.Name.Equals(room.Name), room);
            }
        }

        private  void RoomManager_WaitingRoomDisconnectedPlayer(object sender, RoomQuery roomQuery)
        {
            try
            {
                UnJoinPlayer(roomQuery);
            }
            catch { }
        }
    }
}
