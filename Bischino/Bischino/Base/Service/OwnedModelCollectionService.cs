using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Model;
using Bischino.Base.Model.Attributes;
using Bischino.Base.Security;
using Bischino.Service.Exceptions;

namespace Bischino.Base.Service
{
    public interface IOwnedModelCollectionService<T> : ICollectionService<T> where T : OwnedModel
    {
        Task Remove(string id, IUserBaseIdentity userAuth);
    }
    public class OwnedModelCollectionService<T> : CollectionService<T>, IOwnedModelCollectionService<T> where T : OwnedModel
    {
        public override void ParseModel(T model, IUserBaseIdentity userAuth)
        {
            base.ParseModel(model, userAuth);
            if (!model.OwnersIds.Any(id => id.Equals(userAuth.Id)))
                foreach (var property in AttributesHelper.GetPrivateProperties(typeof(T)))
                    property.SetValue(model, null);
        }

        public async Task Remove(string id, IUserBaseIdentity userAuth)
        {
            var model = await GetById(id);
            if (model == null || !model.OwnersIds.Contains(userAuth.Id))
                throw CollectionException.Default;
            await Remove(id);
        }
    }
}
