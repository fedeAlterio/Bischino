using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Bischino.Skribble;

namespace Skribble.Model
{
    public class Path
    {
        public long RawId { get; set; }
        public string ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public TouchActionType TouchTrackingPoint { get; set; }


        public Path() { }
        public Path(long id, Vector2 pos, TouchActionType touchTrackingPoint)
        {
            (X, Y) = (pos.X, pos.Y);
            RawId = id;
            TouchTrackingPoint = touchTrackingPoint;
        }

        public Path(long id, float x, float y, TouchActionType touchTrackingPoint) : this( id, new Vector2(x,y), touchTrackingPoint) { }
    }
}
