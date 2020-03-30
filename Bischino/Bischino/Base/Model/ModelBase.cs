using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bischino.Base.Model
{
    public abstract class ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }


        public void GenerateNewID()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        public static Dictionary<string, object> PropertiesDictionary<T>(T model, params string[] ignore) where T : ModelBase
        {
            var type = typeof(T);
            var ignoreList = new List<string>(ignore);
            var properties =
                from property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where !ignoreList.Contains(property.Name)
                let val = type.GetProperty(property.Name)?.GetValue(model, null)
                where val != null
                select new { property.Name, Value = val };

            return properties.ToDictionary(prop => prop.Name, prop => prop.Value);
        }
    }
}
