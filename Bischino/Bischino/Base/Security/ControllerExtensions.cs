using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bischino.Security;
using Microsoft.AspNetCore.Mvc;

namespace Bischino.Base.Security
{
    public static class ControllerExtensions
    {
        public static UserIdentity GetIdentity(this ControllerBase me)
        {
            var identity = JwtUserConverter.FromClaimsIdentity(me.HttpContext.User.Identity as ClaimsIdentity);
            return identity;
        }
    }
}
