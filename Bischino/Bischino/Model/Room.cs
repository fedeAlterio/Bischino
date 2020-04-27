using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bischino.Base.Model;
using Bischino.Base.Model.Attributes;

namespace Bischino.Model
{
    public class Room : OwnedModel 
    {
        [Unique]
        [StringLength(30, MinimumLength = 1)]
        public string Name { get; set; }


        [Range(2, 6)]
        public int? MinPlayers { get; set; }


        [Range(2,6)] 
        public int? MaxPlayers { get; set; }


        [StringLength(16, MinimumLength = 1)]
        public string Host { get; set; }

        public DateTime? CreationDate { get; set; }

        public bool IsFull { get; set; }

        
        public bool IsMatchStarted { get; set; }
        public bool? IsPrivate { get; set; }
        public int? RoomNumber { get; set; }



        private List<string> _pendingPlayers;
        public IReadOnlyList<string> PendingPlayers
        {
            get => _pendingPlayers;
            set => _pendingPlayers = value != null ? new List<string>(value) : null;
        }



        private List<string> _notBotPlayers;
        public IReadOnlyList<string> NotBotPlayers
        {
            get => _notBotPlayers;
            set => _notBotPlayers = value != null ? new List<string>(value) : null;
        }



        public int? BotCounter { get; set; }


        public void AddBot()
        {
            BotCounter ??=0;
            _pendingPlayers.Add($"bot{++BotCounter}");
        }


        public void RemoveBot()
        {
            BotCounter??=0;
            _pendingPlayers.Remove($"bot{BotCounter--}");
        }


        public void AddPlayingPlayer(string name)
        {
            _pendingPlayers ??= new List<string>();
            _notBotPlayers ??= new List<string>();

            _notBotPlayers.Add(name);
            _pendingPlayers.Add(name);
        }


        public void RemovePlayingPlayer(string name)
        {
            _notBotPlayers ??= new List<string>();

            _notBotPlayers.Remove(name);
            _pendingPlayers.Remove(name);
        }


        public string GetActualHost() => _pendingPlayers?.FirstOrDefault();
    }
}

