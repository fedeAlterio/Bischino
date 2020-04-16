using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.Controller.Communication.Queries
{
    /// <summary>
    /// This class contains some typical options to filter a query from a collection
    /// </summary>
    public class CollectionQueryOptions
    {
        public static CollectionQueryOptions Default => new CollectionQueryOptions();

        /// <summary>
        /// Number of results the server have to skip 
        /// </summary>
        public int Skip { get; set; } = 0;


        /// <summary>
        /// Maximum number of results the server have to return
        /// </summary>
        public int Limit { get; set; } = 10;


        /// <summary>
        /// Asks the server to return only the results inserted after this field.
        /// </summary>
        public DateTime From { get; set; } = DateTime.MinValue;


        /// <summary>
        /// Asks the server to return only the results inserted before this field.
        /// </summary>
        public DateTime To { get; set; } = DateTime.Now;
    }
}
