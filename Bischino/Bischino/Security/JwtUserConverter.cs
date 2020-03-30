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
        public static Jwt FromModel(User model)
        {
            var claims = new List<Claim>
            {
                new Claim(nameof(User.Id), model.Id),
                new Claim(nameof(User.Username), model.Username)
            };
            var jwt = new Jwt(DateTime.Now.AddDays(1), RoleAttribute.GetRole(typeof(User)), claims);
            return jwt;
        }

        public static UserIdentity FromClaimsIdentity(ClaimsIdentity claimsIdentity)
        {
            var identity = new UserIdentity
            {
                Id = claimsIdentity.FindFirst(nameof(User.Id)).Value,
                Username = claimsIdentity.FindFirst(nameof(User.Username)).Value,
                Role = claimsIdentity.FindFirst(ClaimTypes.Role).Value
            };
            return identity;
        }
    }
}
