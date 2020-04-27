using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bischino.Base.Model.Attributes;
using Bischino.Base.Security;
using Bischino.Model;

namespace Bischino.Security
{
    public class JwtUserConverter
    {
        public static Jwt FromModel(IUserBaseIdentity model)
        {
            var claims = new List<Claim>
            {
                new Claim(nameof(model.Id), model.Id),
            };
            var jwt = new Jwt(DateTime.Now.AddDays(1), RoleAttribute.GetRole(typeof(IUserBaseIdentity)), claims);
            return jwt;
        }

        public static UserIdentity FromClaimsIdentity(ClaimsIdentity claimsIdentity)
        {
            var identity = new UserIdentity
            {
                Id = claimsIdentity.FindFirst(nameof(IUserBaseIdentity.Id)).Value,
                Role = claimsIdentity.FindFirst(ClaimTypes.Role).Value
            };
            return identity;
        }
    }
}
