using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Model;
using Bischino.Base.Model.Attributes;

namespace Bischino.Model
{
    [Role(typeof(User))]
    public class User : UserBase
    {   
        [Unique]
        public string Username { get; set; }
    }
}
