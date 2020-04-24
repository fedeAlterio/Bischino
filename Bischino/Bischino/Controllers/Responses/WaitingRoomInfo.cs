using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Controllers.Responses
{
    public class WaitingRoomInfo
    {
        public IList<string> NotBotPlayers { get; set; }
        public int BotCounter { get; set; }
    }
}
