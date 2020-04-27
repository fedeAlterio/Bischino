using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.CollectionQuery;
using Bischino.Base.Model;

namespace Bischino.Model.Queries
{
    public class Query<T> : CollectionQuery<T> where T : ModelBase
    {
    }
}
