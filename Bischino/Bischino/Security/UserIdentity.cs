using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Security;

namespace Bischino.Security
{
    public class UserIdentity : IUserBaseIdentity
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
    }
}
