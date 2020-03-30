using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Base.CollectionQuery
{
    public class CollectionQueryOptions
    {
        public static CollectionQueryOptions Default => new CollectionQueryOptions();
        
        public int Skip { get; set; } = 0;
        
        public int Limit { get; set; } = 10;

        public DateTime From { get; set; } = DateTime.UnixEpoch;

        public DateTime To { get; set; } = DateTime.Now;
    }
}
