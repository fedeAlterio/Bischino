using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Model.Attributes;

namespace Bischino.Base.Model
{
    public class OwnedModel : ModelBase
    {
        [Private]
        public IList<string> OwnersIds { get; set; }
    }
}
