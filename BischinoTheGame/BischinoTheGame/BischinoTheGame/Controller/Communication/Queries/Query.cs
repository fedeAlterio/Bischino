using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.Controller.Communication.Queries
{
    /// <summary>
    /// This class contains the necessary to make a query to the server to obtain a collection of results of type T
    /// </summary>
    /// <typeparam name="T">Type of data being queried</typeparam>
    public class Query<T> where T : class
    {
        /// <summary>
        /// A empty query. There are no filters on the model.
        /// </summary>
        public static Query<T> Empty => new Query<T> { Model = default, Options = CollectionQueryOptions.Default };


        /// <summary>
        /// Reference model for the server to filter the results.
        /// Every property of every result obtained from this query must be equals to the corresponding in this model
        /// </summary>
        public T Model { get; set; }


        /// <summary>
        /// Options for the query <see cref="CollectionQueryOptions"/>
        /// </summary>
        public CollectionQueryOptions Options { get; set; }
    }
}
