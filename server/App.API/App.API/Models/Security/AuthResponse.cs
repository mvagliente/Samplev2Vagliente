using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.API.Models.Security
{
    public class AuthResponse : AuthResult
    {
        public string id { get; set; }
        public Boolean validate { get; set; }
        public Rol userRol { get; set; }

        public string userName { get; set; }

        public string userEmail { get; set; }

    }

    public class Rol
    {
        public string name { get; set; }
    }
}
