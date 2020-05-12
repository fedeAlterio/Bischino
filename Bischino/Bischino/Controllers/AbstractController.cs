using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Controllers;
using Bischino.Model.Exeptions;
using Microsoft.AspNetCore.Mvc;

namespace Bischino.Controllers
{
    public abstract class AbstractController : ControllerBase
    {
        protected IActionResult TryOk(Action action)
        {
            try
            {
                action.Invoke();
                return Ok();
            }
            catch (GameException e)
            {
                return BadRequest(new ValuedResponse { Message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new ValuedResponse { Message = ErrorDefault });
            }
        }




        protected IActionResult TryValuedOk(Func<object> func)
        {
            try
            {
                var ret = func.Invoke();
                return OkValued(ret);
            }
            catch (GameException e)
            {
                return BadRequest(new ValuedResponse { Message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new ValuedResponse { Message = ErrorDefault });
            }
        }



        protected async Task<IActionResult> TryOkAsync(Func<Task> action)
        {
            try
            {
                await action.Invoke();
                return Ok();
            }
            catch (GameException e)
            {
                return BadRequest(new ValuedResponse { Message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new ValuedResponse { Message = ErrorDefault });
            }
        }




        protected async Task<IActionResult> TryValuedOkAsync<T>(Func<Task<T>> func)
        {
            try
            {
                var ret = await func.Invoke();
                return OkValued(ret);
            }
            catch (GameException e)
            {
                return BadRequest(new ValuedResponse { Message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new ValuedResponse { Message = ErrorDefault });
            }
        }




        protected IActionResult OkValued(object obj) => Ok(new ValuedResponse(obj));
        protected const string ErrorDefault = "An error occurred";
    }
}
