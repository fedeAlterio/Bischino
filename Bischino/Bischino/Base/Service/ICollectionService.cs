using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Bischino.Base.CollectionQuery;
using Bischino.Base.Model;
using Bischino.Base.Security;

namespace Bischino.Base.Service
{
    public interface ICollectionService<T> where T : ModelBase
    {
        Task Create(T model);
        Task<IList<T>> GetAll(CollectionQuery<T> query, IUserBaseIdentity userAuth);
        Task<IList<T>> GetAll(CollectionQuery<T> query);
        Task Update(T model);
        Task Remove(string id);
        Task<IList<T>> GetAll(Expression<Func<T, bool>> expression, CollectionQueryOptions options);
        Task<T> GetById(string id);
        IMongoCollection<T> MongoCollection { get; }
        MongoClient MongoClient { get; }
        void ParseModel(T model, IUserBaseIdentity userAuth);
        void ParseALl(ICollection<T> models, IUserBaseIdentity userAuthorization);
    }
}
