using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Bischino.Base.CollectionQuery;
using Bischino.Base.Model;
using Bischino.Base.Model.Attributes;
using Bischino.Base.Security;
using Bischino.Service.Exceptions;
using Bischino.Settings;

namespace Bischino.Base.Service
{
    public abstract class CollectionService
    {
        protected static MongoClient Client { get; private set; }
        protected static IDatabaseSettings DatabaseSettings { get; private set; }
        protected static IMongoDatabase Database { get; private set; }
        protected static bool MongoDBServiceInitialized { get; set; }

        public static IAggregateFluent<T> AddOptions<T>(IAggregateFluent<T> aggregateFluent, CollectionQueryOptions options)
        {
            if (options is null)
                return aggregateFluent;
            var ret = aggregateFluent.Skip(options.Skip)
                .Limit(options.Limit);
            return ret;
        }

        public static FilterDefinition<V> FilterFromModel<T, V>(T model, string path) where T : ModelBase
        {
            var filter = FilterDefinition<V>.Empty;
            if (model == null)
                return filter;
            var realPath = path == String.Empty ? path : path + ".";
                foreach (var (key, value) in ModelBase.PropertiesDictionary(model))
                filter &= Builders<V>.Filter.Eq(realPath + key, value);

            return filter;
        }

        public static FilterDefinition<V> FilterFromModel<T, V>(T model, Expression<Func<V, T>> expression) where T : ModelBase
        {
            var temp = expression.Body.ToString(); 
            var path = temp.Substring(temp.IndexOf('.') + 1);
            return FilterFromModel<T, V>(model, path);
        }

        public static FilterDefinition<T> FilterFromModel<T>(T model) where T : ModelBase
        {
            return FilterFromModel<T,T>(model, "");
        }

        public static FindOptions<T, T> FindOptionsFromQuery<T>(CollectionQueryOptions queryOptions)
        {
            var ret = new FindOptions<T, T> { Skip = queryOptions.Skip, Limit = queryOptions.Limit };
            return ret;
        }

        public static void InitializeMongoDBService(IDatabaseSettings settings)
        {
            DatabaseSettings = settings;
            Client = new MongoClient(settings.ConnectionString);
            Database = Client.GetDatabase(settings.DatabaseName);
            MongoDBServiceInitialized = true;
        }
    }
    public class CollectionService<T> : CollectionService, ICollectionService<T> where T : ModelBase
    {
        private readonly string _collectionName;
        private IMongoCollection<T> _collection;
        public IMongoCollection<T> MongoCollection => _collection ??= Database.GetCollection<T>(_collectionName);
        public MongoClient MongoClient => Client;

        private void Initialize()
        {
            var names = new List<string>();

            var props = from prop in typeof(T).GetProperties()
                        where Attribute.IsDefined(prop, typeof(UniqueAttribute))
                        select prop;

            foreach (var propertyInfo in props)
            {
                var found = false;
                foreach (var attr in propertyInfo.GetCustomAttributes(true))
                    if (attr is BsonElementAttribute attribute)
                    {
                        names.Add(attribute.ElementName);
                        found = true;
                        break;
                    }
                if (!found)
                    names.Add(propertyInfo.Name);
            }

            foreach (var name in names)
            {
                var options = new CreateIndexOptions { Unique = true };
                var field = new StringFieldDefinition<T>(name);
                var indexDefinition = new IndexKeysDefinitionBuilder<T>().Ascending(field);
                MongoCollection.Indexes.CreateOne(new CreateIndexModel<T>(indexDefinition, options));
            }
        }

        public CollectionService(string collectionName = null)
        {
            if (!MongoDBServiceInitialized)
                throw new CollectionException("CollectionService not initialized");

            _collectionName = collectionName ?? typeof(T).Name;
            Initialize();
        }

        public async Task Create(T model)
        {
            try
            {
                model.GenerateNewID();
                await MongoCollection.InsertOneAsync(model);
            }
            catch (MongoWriteException e)
            {
                throw CollectionException.Parse(e);
            }
            catch
            {
                throw CollectionException.Default;
            };
        }

        protected void ValidateQuery(CollectionQuery<T> query, IUserBaseIdentity userAuth)
        {
            var hiddenProperties = AttributesHelper.GetHiddenProperties(typeof(T), userAuth.Role);
            hiddenProperties = hiddenProperties.Where(p => p.GetValue(query.Model) != null).ToList();

            var isValid = !hiddenProperties.Any();
            if (!isValid)
                throw UnauthorizedException.Default;
        }

        public virtual void ParseModel(T model, IUserBaseIdentity userAuth)
        {
            var toHideProperties = AttributesHelper.GetHiddenProperties(typeof(T), userAuth.Role);
            foreach (var toHideProperty in toHideProperties)
                toHideProperty.SetValue(model, null);
        }

        public void ParseALl(ICollection<T> models, IUserBaseIdentity userAuthorization)
        {
            foreach (var model in models)
                ParseModel(model, userAuthorization);
        }

        private bool IsEmpty(T model)
        {
            var notEmpty =
                from property in typeof(T).GetProperties()
                where property.GetValue(model) != null
                select property;
            return !notEmpty.Any();
        }

        public async Task<IList<T>> GetAll(CollectionQuery<T> query, IUserBaseIdentity userAuth)
        {
            ValidateQuery(query, userAuth);
            var results = await GetAll(query);
            var notEmptyResults = new List<T>();
            foreach (var result in results)
            {
                ParseModel(result, userAuth);
                if (!IsEmpty(result))
                    notEmptyResults.Add(result);
            }
            return notEmptyResults;
        }


        public async Task<IList<T>> GetAll(CollectionQuery<T> query)
        {
            var filter = FilterFromModel(query.Model);
            var findOptions = FindOptionsFromQuery<T>(query.Options);
            var results = await MongoCollection.FindAsync(filter, findOptions);
            var ret = results.ToList();
            return ret;
        }

        public async Task Update(T model)
        {
            await MongoCollection.ReplaceOneAsync(t => t.Id.Equals(model.Id), model);
        }

        public virtual async Task Remove(string id)
        {
            try
            {
                var res = await MongoCollection.DeleteOneAsync(t => t.Id.Equals(id));
                if (res.DeletedCount == 0)
                    throw CollectionException.Default;
            }
            catch
            {
                throw CollectionException.Default;
            }
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression, CollectionQueryOptions options)
        {
            try
            {
                var results = await MongoCollection.FindAsync(expression,
                    new FindOptions<T, T> { Skip = options.Skip, Limit = options.Limit });
                var ret = results.ToList();
                return ret;
            }
            catch (Exception)
            {
                throw CollectionException.Default;
            }
        }

        public async Task<T> GetById(string id)
        {
            var results = await GetAll(model => model.Id.Equals(id), CollectionQueryOptions.Default);
            return results.FirstOrDefault();
        }

    }
}
