using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Model;

namespace Bischino.Base.CollectionQuery
{
    public class CollectionQuery<T> where T : ModelBase
    {
        public T Model { get; set; }
        public CollectionQueryOptions Options { get; set; }
    }
}
