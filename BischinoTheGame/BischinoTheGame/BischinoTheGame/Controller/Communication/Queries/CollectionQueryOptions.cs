using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.Controller.Communication.Queries
{
    public class CollectionQueryOptions
    {
        public static CollectionQueryOptions Default => new CollectionQueryOptions();
        public int Skip { get; set; } = 0;
        public int Limit { get; set; } = 10;

        public DateTime From { get; set; } = DateTime.MinValue;

        public DateTime To { get; set; } = DateTime.Now;
    }
}
