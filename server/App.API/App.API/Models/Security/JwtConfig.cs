using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.API.Models.Security
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public TimeSpan ExpiryTimeFrame { get; internal set; }
    }
}
