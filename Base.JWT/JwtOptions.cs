using System;
using System.Collections.Generic;
using System.Text;

namespace Base.JWT
{
    public class JwtOptions
    {
        public string Audience { get; set; }
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public int Expires { get; set; }
    }
}
