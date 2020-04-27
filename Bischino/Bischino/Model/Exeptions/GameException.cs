using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Model.Exeptions
{
    public class GameException : Exception
    {
        public GameException(string message) : base(message)
        {
            
        }
    }
}
