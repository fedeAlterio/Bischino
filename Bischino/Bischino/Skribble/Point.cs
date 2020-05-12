using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Skribble
{
    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point() { }
        public Point(float x, float y)
        {
            (X, Y) = (x, y);
        }
    }
}
