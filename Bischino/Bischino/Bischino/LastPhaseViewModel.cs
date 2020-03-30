using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Bischino
{
    public class LastPhaseViewModel
    {
        public IList<Card> Cards { get; set; }
        public bool CanBetWin { get; set; }
        public bool CanBetLose { get; set; }
    }
}
