using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bischino.Base.Model;
using Bischino.Base.CollectionQuery;
using Bischino.Base.Security;
using Bischino.Base.Service;
using Bischino.Model;
using Bischino.Security;
using Bischino.Service;
using Bischino.Service.Exceptions;

namespace Bischino.Base.Controllers
{
    public abstract class ModelController<T> : Controller where T : ModelBase
    {
        protected readonly ICollectionService<T> MongoService;
        
        protected ModelController(ICollectionService<T> collectionService)
        {
            MongoService = collectionService;
        }
        
        protected async Task<IActionResult> CreateModel(T model)
        {
            try
            {
                await MongoService.Create(model);
                return Ok();
            }
            catch (CollectionException e)
            {
                return BadRequest(new ServerResponse{Message = e.Message});
            }
            catch (Exception)
            {
                return BadRequest(ServerResponse.Error);
            }
        }

        protected async Task<IActionResult> RemoveModel(string id)
        {
            try
            {
                await MongoService.Remove(id);
                return Ok();
            }
            catch (UnauthorizedException)
            {
                return Unauthorized(ServerResponse.Error);
            }
            catch (Exception)
            {
                return BadRequest(ServerResponse.Error);
            }
        }

        protected async Task<IActionResult> UpdateModel(T model)
        {
            try
            {
                await MongoService.Update(model);
                return Ok();
            }
            catch (UnauthorizedException)
            {
                return Unauthorized(ServerResponse.Error);
            }
            catch (Exception)
            {
                return BadRequest(ServerResponse.Error);
            }
        }


        private string GetToken()
        {
            var strings = from strSeq in HttpContext.Request.Headers.Values
                          let str = strSeq.ToString()
                          where str.Contains("Bearer")
                          select str.Split(' ')[1];
            return strings.FirstOrDefault();
        }

        protected async Task<IActionResult> GetAllModels(CollectionQuery<T> query)
        {
            try
            {
                var identity = this.GetIdentity();
                var results = await MongoService.GetAll(query, identity);
                return Ok(new ValuedResponse(results));
            }
            catch (UnauthorizedException)
            {
                return Unauthorized(ServerResponse.Unauthorized);
            }
            catch (Exception e)
            {
                return BadRequest(ServerResponse.Error);
            }
        }
    }
}
