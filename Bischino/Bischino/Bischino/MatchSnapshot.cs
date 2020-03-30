using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Bischino
{
    public class MatchSnapshot
    {
        public Player Player { get; set; }
        public IList<Card> DroppedCards { get; set; }
        public IList<PrivatePlayer> OtherPlayers { get; set; }
        public bool IsMatchEnded { get; set; }
        public bool IsEndTurn { get; set; }
        public bool IsPhaseEnded { get; set; }
        public string Host { get; set; }
        public PrivatePlayer PlayerTurn { get; set; }
        public string DisconnectedPlayer { get; set; }
    }
}
