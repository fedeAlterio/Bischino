using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Base.Security
{
    public interface IUserBaseIdentity
    {
        string Id { get; set; }
        string Role { get; set; }
    }
}
