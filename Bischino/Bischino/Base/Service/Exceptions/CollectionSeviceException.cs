using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Bischino.Service.Exceptions
{
    public class CollectionException : Exception
    {
        public static CollectionException Default => new CollectionException("An error occurred");
        public static CollectionException Duplicate => new CollectionException("Duplicate key");
        public static CollectionException NotFound(string key = "key") => new CollectionException($"{key} not found");
        public static CollectionException Parse(MongoWriteException e) => e.WriteError switch
        {
            { Category: ServerErrorCategory.DuplicateKey } => Duplicate,
            _ => Default
        };
        public CollectionException(string message) : base(message)
        {

        }
    }

    public class UnauthorizedException : CollectionException
    {
        public new static UnauthorizedException Default => new UnauthorizedException("Unauthorized");
        public UnauthorizedException(string message) : base(message)
        {
        }
    }

}
