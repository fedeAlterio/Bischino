using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Model.Attributes;

namespace Bischino.Base.Model
{
    public abstract class UserBase : ModelBase
    {
        [Private]
        public override string Id { get; set; }
    }
}
