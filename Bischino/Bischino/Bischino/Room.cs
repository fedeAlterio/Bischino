using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bischino.Base.Model;
using Bischino.Base.Model.Attributes;
using Bischino.Bischino;

namespace Bischino.Bischino
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

        public IList<string> PendingPlayers { get; set; }


        public bool IsMatchStarted { get; set; }
    }
}
