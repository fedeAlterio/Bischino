using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Model
{
    public class Card
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public bool IsPaolo { get; set; }
        public string Owner { get; set; }
    }
}
