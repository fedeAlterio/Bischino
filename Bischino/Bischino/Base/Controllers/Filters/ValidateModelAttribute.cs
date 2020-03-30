using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bischino.Base.Controllers.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is Controller controller && !controller.ModelState.IsValid)
            {
                var errorMessage = string.Join("'\n", filterContext.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                var result =
                    new JsonResult(new ValuedResponse {Message = errorMessage}) {StatusCode = (int) HttpStatusCode.BadRequest};

                filterContext.Result = result;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
