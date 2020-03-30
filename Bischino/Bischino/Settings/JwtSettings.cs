using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Settings
{
    public class JwtSettings
    {
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
