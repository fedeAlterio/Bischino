using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skribble.Model;

namespace Bischino.Skribble
{
    public class Player
    {
        private List<Path> _draw = new List<Path>();
        public IReadOnlyList<Path> Draw => _draw;


        public string Username { get; set; }



        public Player(string username)
        {
            Username = username;
        }



        public void UpdateDraw(IEnumerable<Path> paths)
        {
            _draw.AddRange(paths);
        }
    }
}
