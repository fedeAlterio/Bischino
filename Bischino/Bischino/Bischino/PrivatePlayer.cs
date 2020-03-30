﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Bischino
{
    public class PrivatePlayer
    {
        public string Name { get; set; }
        public int CardCount { get; set; }
        public int? WinBet { get; set; }
        public int? PhaseWin { get; set; }
        public int? TotLost { get; set; }
        public bool HasLost { get; set; }

        public static PrivatePlayer FromPlayer(Player player)
        {
            var ret = new PrivatePlayer
            {
                Name = player.Name,
                CardCount = player.Cards?.Count ?? 0,
                WinBet = player.WinBet,
                HasLost = player.HasLost,
                TotLost = player.TotLost,
                PhaseWin = player.PhaseWin
            };

            return ret;
        }
    }
}
