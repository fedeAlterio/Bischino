using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Skribble
{
    public class Line
    {
        public Point Start { get; set; }
        public IList<Point> Points { get; set; } = new List<Point>();
        public string ID { get; set; }


        public Line() { }
        public Line(Point start)
        {
            Start = start;
        }


        public void AddPoint(Point point) => Points.Add(point);
    }
}
