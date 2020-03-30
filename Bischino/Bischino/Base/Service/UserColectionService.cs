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
    public interface IUserCollectionService<T> : ICollectionService<T> where T : UserBase
    {
        Task<T> GetUser(IUserBaseIdentity userAuth);
    }

    public class UserCollectionService<T> : CollectionService<T>, IUserCollectionService<T> where T : UserBase
    {
        public Task<T> GetUser(IUserBaseIdentity userAuth)
        {
            var ret = GetById(userAuth.Id);
            return ret;
        }

        public override void ParseModel(T model, IUserBaseIdentity userAuth)
        {
            var toHideProperties = AttributesHelper.GetHiddenProperties(typeof(T), userAuth.Role);
            if (!userAuth.Role.Equals(RoleAttribute.GetRole(typeof(T))) || !model.Id.Equals(userAuth.Id))
                foreach (var privateProp in AttributesHelper.GetPrivateProperties(typeof(T)))
                    privateProp.SetValue(model, null);
            foreach (var toHideProperty in toHideProperties)
                toHideProperty.SetValue(model, null);
        }
    }
}
