using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Bischino.Base.CollectionQuery;
using Bischino.Base.Model;
using Bischino.Base.Model.Attributes;
using Bischino.Base.Security;
using Bischino.Base.Service;
using Bischino.Model;
using Bischino.Service;

namespace Bischino.Base.Controllers
{
    public class ValidationException : Exception
    {
        public static ValidationException Default => new ValidationException("Authentication error");
        public ValidationException(string message = "") : base(message)
        {
        }
    }
    public abstract class UserBaseController<T> : ModelController<T> where T : UserBase
    {
        public IUserCollectionService<T> UserCollectionService { get; }
        protected abstract Jwt GetJwt (T model);
        protected abstract IUserBaseIdentity BaseIdentity { get; }

        protected abstract Expression<Func<T, bool>> CredentialsCheckExpression(T user);

        protected async Task<T> GetUserByIdentity()
        {
            var identity = this.BaseIdentity;
            var user = await MongoService.GetById(identity.Id);
            return user;
        }

        protected async Task<IActionResult> LoginBase(T user)
        {
            //todo
            try
            {
                var results = await MongoService.GetAll(CredentialsCheckExpression(user), new CollectionQueryOptions());
                var collectionUser = results.FirstOrDefault();
                if(collectionUser == null)
                    throw ValidationException.Default;

                var jwt = GetJwt(collectionUser);
                //var loginData = null;//; new LoginData<T> {Token = jwt.TokenString, LoggedUser = collectionUser};
                return Ok(new ValuedResponse(null));
            }
            catch (ValidationException e)
            {
                return Unauthorized(new ValuedResponse {Message = e.Message});
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        protected UserBaseController(IUserCollectionService<T> collectionService) : base(collectionService)
        {
            UserCollectionService = collectionService;
        }
    }
}