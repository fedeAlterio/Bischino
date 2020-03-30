using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.Controller.Communication.Queries
{
    public class Query<T>
    {
        public static Query<T> Empty => new Query<T> { Model = default, Options = CollectionQueryOptions.Default };
        public T Model { get; set; }
        public CollectionQueryOptions Options { get; set; }
    }
}
